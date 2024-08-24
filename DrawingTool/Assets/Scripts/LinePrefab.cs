using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

    public class LinePrefab : MonoBehaviour
    {
        [SerializeField] private  List<Material> materialList = null;

        [SerializeField] public GameObject indexObj = null;

        [SerializeField] public Image indexImage = null;

        [SerializeField] public Text indexText = null;

        [SerializeField] public UnityAction lineEvent = null;
                       
        [SerializeField] public GameObject indexPanel = null;

        [SerializeField] public LineRenderer lineRenderer = null;


        public RectTransform RectTransform => this.gameObject.GetComponent<RectTransform>();


        public int prefabIndex;
        public void SetIndex()
        {
            
            indexText.text = (prefabIndex + 1).ToString();
        }
  
        public void SetPos(Vector2 xy)
        {
            indexObj.SetActive(true);            
            indexObj.transform.SetParent(indexPanel.transform);
            indexObj.transform.localPosition = new Vector3(xy.x, xy.y, indexPanel.transform.position.z);
        indexObj.transform.localScale = Vector3.one;
        }
        public void SetBackGroundColor(bool active)
        {
            if (active == true)
            {
                this.GetComponent<Renderer>().material = materialList[0]; //.color = new Color(110 / 255f, 255 / 255f, 0 / 255f);
                this.indexImage.color = Color.green;
               

            }
            else
            {
                this.GetComponent<Renderer>().material = materialList[1];
                this.indexImage.color = new Color(217 / 255f, 217 / 255f, 217 / 255f);


            }
        }

        public void MoveRect(bool One)
        {
            if (One == true)
            {
                RectTransform.anchoredPosition = new Vector3(0, 0, -0.1f);
            }
            else
            { 
                RectTransform.anchoredPosition = new Vector3(0, 0, 0);
            }
        }


    }
