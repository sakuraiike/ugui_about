using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ImageObj : MonoBehaviour,IInitializePotentialDragHandler,IBeginDragHandler,IDragHandler,IEndDragHandler {
	public Vector2 pos;
	public Vector3[] screenPos=new Vector3[4];
	public Color color=Color.red;
	private  Vector2 previousPoint;
	public  RawImage childRawImage;

	public float  lineWidth=5;

	private Texture2D _tex;
	public Texture2D tex{
		get{
			if(_tex==null){
			Texture2D t= this.GetComponent<RawImage>().texture as Texture2D;
				_tex=new Texture2D(t.width,t.height,TextureFormat.ARGB32,false);
				for(int i=0;i<_tex.height;i++){
					for(int j=0;j<_tex.width;j++){

						_tex.SetPixel(j,i,new Color(1,1,1,0));
					}

				}
				this.childRawImage.texture=_tex;
				this.childRawImage.gameObject.SetActive(true);
			}
			return _tex;
		}

	}
	public void OnInitializePotentialDrag (PointerEventData eventData){
		pos=	LimitedPos(eventData.position);
	}		

	public void OnBeginDrag (PointerEventData eventData){

		pos=	LimitedPos(eventData.position);
		this.previousPoint=pos;

	}
	public void OnDrag (PointerEventData eventData){
		pos=	LimitedPos(eventData.position);
	//	if(pos!=previousPoint){


			ModifyPixel(previousPoint-new Vector2(screenPos[0].x,screenPos[0].y),pos-new Vector2(screenPos[0].x,screenPos[0].y));

		this.previousPoint=pos;
//		}
	}
	public void OnEndDrag (PointerEventData eventData){

		pos=	LimitedPos(eventData.position);

	}
	public Vector2 currentSize{
		get{
		return this.rectTransform.sizeDelta;
		}
	}
	public RectTransform rectTransform{
		get{
			return this.GetComponent<RectTransform>();
		}

	}
	public Vector2 LimitedPos(Vector2 pos){
		this.rectTransform.GetWorldCorners(this.screenPos);
		if(pos.x<screenPos[0].x)pos.x=screenPos[0].x;
		if(pos.x>screenPos[2].x)pos.x=screenPos[2].x;
		if(pos.y<screenPos[0].y)pos.y=screenPos[0].y;
		if(pos.y>screenPos[2].y)pos.y=screenPos[2].y;
		return pos;
	}
	public void ModifyPixel(Vector2 start,Vector2 end){
		float startxRatio=start.x*1.0f/currentSize.x;
		float startyRation=start.y*1.0f/currentSize.y;

		float endxRatio=end.x*1.0f/currentSize.x;
		float endyRatio=end.y*1.0f/currentSize.y;

		Vector2 startPixel=new Vector2(startxRatio*tex.width,startyRation*tex.height);
		Vector2 endPixel=new Vector2(endxRatio*tex.width,endyRatio*tex.height);
		//求斜率 求 b 
		float k=(endPixel.y-startPixel.y)*1.0f/(endPixel.x-startPixel.x);
		float b=startPixel.y- startPixel.x*k;

		for(int i=0;i<tex.height;i++){
			for(int j=0;j<tex.width;j++){
				if(j<Mathf.Min(startPixel.x,endPixel.x)-lineWidth||j>Mathf.Max(startPixel.x,endPixel.x)+lineWidth||i<Mathf.Min(startPixel.y,endPixel.y)-lineWidth||i>Mathf.Max(startPixel.y,endPixel.y)+lineWidth){
					continue;
				}
				if(Mathf.Abs(k*j-i+b)/Mathf.Sqrt(k*k+1)<lineWidth){

					tex.SetPixel(j,i,this.color);
				
				}

			}

		}
		tex.Apply();

	}
}
	/*

	// Use this for initialization
	//void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/