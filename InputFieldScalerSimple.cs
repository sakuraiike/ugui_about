using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(InputField))]
public class InputFieldScalerSimple : MonoBehaviour {
    [Tooltip("是否以原始大小为最小值")]
    public bool keepOriginalMinSize=true;
    private Vector2 originalSize;
    [Tooltip("当前大小")]
    public Vector2 currentSize;
    private InputField _inputField;
    private InputField inputField{
        get{
            return _inputField??(_inputField=this.GetComponent<InputField>());
        }
    }
    public void OnValueChanged(string value){

        if(this.keepOriginalMinSize)
            this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)0, Mathf.Max(originalSize.x, LayoutUtility.GetPreferredSize(this.rectTransform, 0)+20));
        else
            this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)0,LayoutUtility.GetPreferredSize(this.rectTransform, 0)+20);
        
        this.currentSize=this.rectTransform.sizeDelta;

    }
    void Awake(){
        this.originalSize=this.rectTransform.sizeDelta;
        this.currentSize=this.rectTransform.sizeDelta;
    }

    private RectTransform _rectTransform;
    public RectTransform rectTransform{
        get{
            return _rectTransform??(_rectTransform=this.GetComponent<RectTransform>());
        }
    }
    void OnEnable(){

        inputField.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged("");
    }
   //使得该组件失效
    void OnDisable(){
        inputField.onValueChanged.RemoveListener(OnValueChanged);

        this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)0, originalSize.x);

    }
}
