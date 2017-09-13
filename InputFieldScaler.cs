using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InputField))]
public class InputFieldScaler:MonoBehaviour,ILayoutElement
{
    private Text textComponent
    {
        get
        {
            return this.GetComponent<InputField>().textComponent;
        }

    }

    public TextGenerationSettings GetTextGenerationSettings(Vector2 extents)
    {

        var settings = textComponent.GetGenerationSettings(extents);
        settings.generateOutOfBounds = true;

        return settings;
     

    }

    private RectTransform m_Rect;

    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    public void OnValueChanged(string v)
    {

        rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)0, LayoutUtility.GetPreferredSize(m_Rect, 0));
    }

    void OnEnable()
    {

        this.inputField.onValueChanged.AddListener(OnValueChanged);
    }

    void OnDisable()
    {

    }

    private Vector2 originalSize;

    public ContentSizeFitter filter
    {

        get
        {
            return this.GetComponent<ContentSizeFitter>();
        }
    }

    private InputField _inputField;

    public InputField inputField
    {
        get
        {

            return _inputField ?? (_inputField = this.GetComponent<InputField>());

        }

    }

    protected  void Awake()
    {
        this.originalSize = this.GetComponent<RectTransform>().sizeDelta;
       

    }

    private string text
    {
        get
        {

            return this.GetComponent<InputField>().text;
        }

    }

    private TextGenerator _generatorForLayout;

    public TextGenerator generatorForLayout
    {

        get
        {
            return _generatorForLayout ?? (_generatorForLayout = new TextGenerator());
        }
    }

    [Tooltip("测试用")]
    public float Width;
    [Tooltip("测试用")]
    public float Height;

    public void Update()
    {
        
        this.Width = this.preferredWidth;
        this.Height = this.preferredHeight;
    }


    public float preferredWidth
    {
        get
        {
            if(keepMinOriginalSize){

                return Mathf.Max(this.originalSize.x, generatorForLayout.GetPreferredWidth(text, GetTextGenerationSettings(Vector2.zero)) / textComponent.pixelsPerUnit + 20);

            }else{
                return generatorForLayout.GetPreferredWidth(text, GetTextGenerationSettings(Vector2.zero)) / textComponent.pixelsPerUnit + 20;
            }
            
           
        }

    }

	
	
    public virtual void CalculateLayoutInputHorizontal()
    {
    }

    public virtual void CalculateLayoutInputVertical()
    {
    }

    public virtual float minWidth
    {
        get { return -1; }
    }



    public virtual float flexibleWidth { get { return -1; } }

    public virtual float minHeight
    {
        get { return -1; }
    }

    public virtual float preferredHeight
    {
        get
        {
            if(keepMinOriginalSize){
                return Mathf.Max(this.originalSize.y, generatorForLayout.GetPreferredHeight(text, GetTextGenerationSettings(new Vector2(this.textComponent.GetPixelAdjustedRect().size.x, 0.0f))) / textComponent.pixelsPerUnit);

            }else{

                return generatorForLayout.GetPreferredHeight(text, GetTextGenerationSettings(new Vector2(this.textComponent.GetPixelAdjustedRect().size.x, 0.0f))) / textComponent.pixelsPerUnit;
            }
            
        }
    }

    public virtual float flexibleHeight { get { return -1; } }

    [SerializeField][Tooltip("缩放的最小值为 Awake()中取得的值")]
    private bool keepMinOriginalSize=true;

    [SerializeField][Tooltip("提高Layou计算优先级，要比InputField大 这里设为1")]
    private int priority=1;

    public virtual int layoutPriority { get { return priority; } }



}


/*
    [TextArea(3,4)][SerializeField]
    protected string m_Text=string.Empty;
    public FontData _fontData;
    public TextGenerator generator;
    public TextGenerator generatorForLayout;
    public TextGenerationSettings settings;

    protected static Material m_DefaultText=null;
    protected bool m_DisableFontTextureRebuildCallback=false;

    // Use this for initialization
    void Start () {
       
    }
    
    // Update is called once per frame
    void Update () {
        
    }
    public Color color;
    public float pixelsPerUnit{
        get{
            Canvas canvas=this.GetComponent<Graphic>().canvas;
            if(!canvas)
                return 1;
            if(!_fontData.font||_fontData.font.dynamic)
                return canvas.scaleFactor;

            if(_fontData.fontSize<=0)
                return 1;

            return _fontData.fontSize/(float)_fontData.fontSize;

        }

    }
    public TextGenerationSettings GetGenerationSettings(Vector2 extents){
        var settings=new TextGenerationSettings();
        settings.generationExtents=extents;
        if(_fontData.font!=null&&_fontData.font.dynamic){
            settings.fontSize=_fontData.fontSize;
            settings.resizeTextMinSize=_fontData.minSize;
            settings.resizeTextMaxSize=_fontData.maxSize;
        }
        settings.textAnchor=_fontData.alignment;
        settings.alignByGeometry=_fontData.alignByGeometry;
        settings.font=_fontData.font;
        settings.richText=_fontData.richText;
        settings.lineSpacing=_fontData.lineSpacing;
        settings.fontStyle=_fontData.fontStyle;
        settings.resizeTextForBestFit=_fontData.bestFit;
        settings.horizontalOverflow=_fontData.horizontalOverflow;
        settings.verticalOverflow=_fontData.verticalOverflow;

        settings.scaleFactor=pixelsPerUnit;
        settings.color=color;
        settings.pivot=this.GetComponent<RectTransform>().pivot;
        settings.updateBounds=false;


        return settings;
    }
    readonly UIVertex[] _TempVerts=new UIVertex[4];

    public void OnPopulateMesh(VertexHelper toFill){
        if(_fontData.font=null)return;

        m_DisableFontTextureRebuildCallback=true;

        Vector2 extents=this.GetComponent<RectTransform>().rect.size;

        var settings=GetGenerationSettings(extents);

        generator.PopulateWithErrors(this.m_Text,settings,gameObject);

        IList<UIVertex> verts=generator.verts;
        float unitsPerPixel=1/pixelsPerUnit;

        int vertCount=verts.Count-4;

        Vector2 roundingOffset=new Vector2(verts[0].position.x,verts[0].position.y)*unitsPerPixel;

        roundingOffset=this.GetComponent<Graphic>().PixelAdjustPoint(roundingOffset)-roundingOffset;

        toFill.Clear();

        if(roundingOffset!=Vector2.zero){
            for(int i=0;i<vertCount;i++){
                int tempVertsIndex=i&3;
                _TempVerts[tempVertsIndex]=verts[i];
                _TempVerts[tempVertsIndex].position*=unitsPerPixel;
                _TempVerts[tempVertsIndex].position.x+=roundingOffset.x;
                _TempVerts[tempVertsIndex].position.y+=roundingOffset.y;
                if(tempVertsIndex==3)
                    toFill.AddUIVertexQuad(_TempVerts);

            }

        }else{
            for(int i=0;i<vertCount;i++){
                int tempVertsIndex=i&3;
                _TempVerts[tempVertsIndex]=verts[i];
                _TempVerts[tempVertsIndex].position*=unitsPerPixel;
                if(tempVertsIndex==3)
                    toFill.AddUIVertexQuad(_TempVerts);

            }


        }
        m_DisableFontTextureRebuildCallback=false;



    }
*/


/*

            //
    // Constructors
    //
    protected UIBehaviour();

    //
    // Methods
    //
    protected virtual void Awake();

    public virtual bool IsActive();

    public bool IsDestroyed();

    protected virtual void OnBeforeTransformParentChanged();

    protected virtual void OnCanvasGroupChanged();

    protected virtual void OnCanvasHierarchyChanged();

    protected virtual void OnDestroy();

    protected virtual void OnDidApplyAnimationProperties();

    protected virtual void OnDisable();

    protected virtual void OnEnable();

    protected virtual void OnRectTransformDimensionsChange();

    protected virtual void OnTransformParentChanged();

    protected virtual void OnValidate();

    protected virtual void Reset();

    protected virtual void Start();


    */
//}
