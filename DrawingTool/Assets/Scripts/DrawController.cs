using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;

namespace ggyusang{ 
    public class DrawController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] private LineRenderer lineRenderer = null;

        [SerializeField] public LinePrefab drawingPrefab = null;
        [SerializeField] private GameObject zero = null;
        [SerializeField] private GameObject max = null;

        [SerializeField] private float zPos;
        [SerializeField] private float startWidth;
        [SerializeField] private float endWidth;
        [SerializeField] private float limit;
        [SerializeField] private int limitMinSize;

        [SerializeField] private Vector2[] openCvPos;

        private Texture2D thumb = null;
        private Texture2D mask = null;



        private int positionCount;

        private Vector3 firstTouch;
        private Vector3 lastTouch;

        private Vector3[] positions;


        [SerializeField] public List<LinePrefab> lineObj = null;

        private bool isOnPointer = false;
        private int index;

        public float xgap;
        public float ygap;

        


		private void Start()
        {
            Input.multiTouchEnabled = false;
            lineObj = new List<LinePrefab>();  
         }





        void FreeDraw()
        {

            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zPos);
          
            lineRenderer.positionCount++;
            positionCount = lineRenderer.positionCount;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(Camera.main.ScreenToWorldPoint(mousePos).x, Camera.main.ScreenToWorldPoint(mousePos).y, zPos));

        }


        public void OnBeginDrag(PointerEventData eventData)
        {

            LinePrefab drawing = Instantiate(drawingPrefab);
            lineRenderer = drawing.GetComponent<LineRenderer>();
            firstTouch = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zPos);
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = startWidth;
            this.lineObj.Add(drawing);
            index = lineObj.Count - 1;
            isOnPointer = true;

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isOnPointer == true)
            {
                FreeDraw();
            }

        }
        public void HideAndOn()
        {
            HideLine();
            Invoke("ShowLineCall", 3f);
         
        }
        public void HideLine()
        {
			for (int i = 0; i < lineObj.Count; i++)
			{
                lineObj[i].gameObject.SetActive(false);
                lineObj[i].indexObj.SetActive(false);
			}
		}

 




        public void LineRenderActive(bool active)
        {
            for (int i = 0; i < lineObj.Count; i++)
            {
                lineObj[i].lineRenderer.enabled = active;
                lineObj[i].indexObj.SetActive(active);
            }
        }


        public void ShowLine()
        {
            for (int i = 0; i < lineObj.Count; i++)
            {
                lineObj[i].gameObject.SetActive(true);
                lineObj[i].indexObj.SetActive(true);
               
            }
        }
        public void ShowLineCall()
        {
            for (int i = 0; i < lineObj.Count; i++)
            {
                lineObj[i].gameObject.SetActive(true);
                lineObj[i].indexObj.SetActive(true);

            }

        }
        public void DestroyDrawAll()
        {
            int count = lineObj.Count;
			for (int i = 0; i < count; i++)
			{
                Destroy(lineObj[0].gameObject);
                lineObj.Remove(lineObj[0]);
            }
		}
        public void DestroyDraw()
        {
            Destroy(lineObj[this.index].gameObject);
            lineObj.Remove(lineObj[index]);
        }
        public void DestroyDraw(int index)
        {
            if (lineObj.Count <= index)
            {
                return;
            }

            Destroy(lineObj[index].indexObj);
            Destroy(lineObj[index].gameObject);
            lineObj.Remove(lineObj[index]);
            RefreshIndex();
        }

        public void RefreshIndex()
        {
            for (int i = 0; i < lineObj.Count; i++)
            {
                lineObj[i].prefabIndex = i;
                lineObj[i].SetIndex();
            }
        }
     
        public void SetLineBackGround(int index,bool active)
        {
            if (lineObj.Count <= index)
            {
                return;
            }

            lineObj[index].SetBackGroundColor(active);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            xgap =720/ (Camera.main.WorldToScreenPoint(max.transform.position).x - Camera.main.WorldToScreenPoint(zero.transform.position).x);
            ygap = 720/ (Camera.main.WorldToScreenPoint(zero.transform.position).y - Camera.main.WorldToScreenPoint(max.transform.position).y);
            isOnPointer = false;
            positions = new Vector3[positionCount];
            openCvPos = new Vector2[positionCount];
            lastTouch = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zPos);
            if (Mathf.Abs(lastTouch.x - firstTouch.x) + Mathf.Abs(lastTouch.y - firstTouch.y) > limit)
            {
                DestroyDraw();
                return;
            }
            if (positionCount < limitMinSize)
            {
                DestroyDraw();
                return;
            }


            lineRenderer.GetPositions(positions);

            for (int i = 0; i < positions.Length; i++)
            {
                openCvPos[i] = new Vector2(
                    (Camera.main.WorldToScreenPoint(positions[i]).x - Camera.main.WorldToScreenPoint(zero.transform.position).x)*xgap, 
                    (Camera.main.WorldToScreenPoint(zero.transform.position).y - Camera.main.WorldToScreenPoint(positions[i]).y)*ygap
                    );
       
            }
            RefreshIndex();
            lineObj[index].SetPos(new Vector2(firstTouch.x*xgap, firstTouch.y*ygap));
           // Debug.Log(lineObj[i]);
         //   MakeImage();

        }






    /*    public void MakeImage()
        {
            var matMask = new Mat(720, 720, CvType.CV_8UC3, new Scalar(0, 0, 0));
            var points = new List<Point>();
            this.openCvPos.ToList().ForEach(position => points.Add(new Point((int)position.x, (int)position.y)));
            var ointsMat = new MatOfPoint(points.ToArray());

            var hullPoints = new List<MatOfPoint>();
            hullPoints.Add(ointsMat);

            var matThumb = matMask.clone();
            Imgproc.drawContours(matMask, hullPoints, -1, new Scalar(255, 255, 255), -1);
            Imgproc.cvtColor(matMask, matMask, Imgproc.COLOR_BGR2RGB);

            Imgproc.drawContours(matThumb, hullPoints, -1, new Scalar(255, 255, 255), 2);
            Imgproc.cvtColor(matThumb, matThumb, Imgproc.COLOR_BGR2RGB);

            var rect = Imgproc.boundingRect(ointsMat);
            var matResult = matThumb.submat(rect);

            var matThumbOneChannel = new Mat(matResult.size(), CvType.CV_8UC1);
            Imgproc.cvtColor(matResult, matThumbOneChannel, Imgproc.COLOR_RGB2GRAY);
            var matChannels = matResult.Split();
            matChannels.Add(matThumbOneChannel);
            var matThumbResult = matChannels.Merge();
            
            /// 썸네일
            var thumbResult = matThumbResult.ToTexture2D(true, 0);
            /// 마스크
            var resultTexture = matMask.ToTexture2D(true, 0);
            
            this.thumb = thumbResult;
            this.mask = resultTexture;

            matMask.Dispose();
            ointsMat.Dispose();
            hullPoints.ForEach(point => point?.Dispose());
            matThumb.Dispose();
            matResult.Dispose();
            matThumbOneChannel.Dispose();
            matChannels.ForEach(channel => channel?.Dispose());
            matThumbResult.Dispose();
        }
*/
        public void OnPointerExit(PointerEventData eventData)
        {
            isOnPointer = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isOnPointer = true;
        }
        private void OnDestroy()
        {
            this.DestroyDrawAll();
        }


    }
    }