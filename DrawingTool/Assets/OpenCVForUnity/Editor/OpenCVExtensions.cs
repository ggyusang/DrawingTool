using System.Collections;
using System.Collections.Generic;

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using UnityEngine;

namespace ggyusang
{
    public static class OpenCVExtensions
    {
        public static Texture2D ToTexture2D(this Mat matSource, int flipCode)
        {
            var textureFormat = TextureFormat.RGBA32;
            var type = matSource.type();
            if (type == CvType.CV_8UC3)
            {
                textureFormat = TextureFormat.RGB24;
            }

            var cloned = matSource.clone();
            Core.flip(cloned, cloned, flipCode);
            var texture = new Texture2D(cloned.width(), cloned.height(), textureFormat, false);
            OpenCVForUnity.UnityUtils.Utils.matToTexture2D(cloned, texture);
            cloned.Dispose();
            return texture;
        }

        public static Texture2D ToTexture2D(this Mat target, bool flip, int flipCode)
        {
            var format = target.type() == CvType.CV_8UC3 ? TextureFormat.RGB24 : TextureFormat.RGBA32;
            var cloned = target.clone();
            var texture = new Texture2D(cloned.width(), cloned.height(), format, false);
            OpenCVForUnity.UnityUtils.Utils.matToTexture2D(cloned, texture, flip, flipCode);
            cloned.Dispose();
            return texture;
        }

        public static Texture2D ToTexture2D(this Mat matSource, bool dispose = false)
        {
            var textureFormat = TextureFormat.RGBA32;
            var type = matSource.type();
            if (type == CvType.CV_8UC3)
            {
                textureFormat = TextureFormat.RGB24;
            }

            if (dispose)
            {
                var texture = matSource.ToTexture2D(textureFormat);
                matSource.Dispose();
                return texture;
            }
            else
            {
                return matSource.ToTexture2D(textureFormat);
            }
        }

        public static Texture2D ToTexture2D(this Mat matSource, TextureFormat textureFormat)
        {
            var texture = new Texture2D(matSource.width(), matSource.height(), textureFormat, false);
            OpenCVForUnity.UnityUtils.Utils.matToTexture2D(matSource, texture);
            return texture;
        }

        public static List<Mat> Split(this Mat matSource)
        {
            var channels = new List<Mat>();
            Core.split(matSource, channels);
            return channels;
        }

        public static Mat Merge(this List<Mat> list)
        {
            var dest = new Mat();
            Core.merge(list, dest);
            return dest;
        }

        public static Mat ConvertToColor(this Mat matSource, int code)
        {
            var dest = new Mat();
            Imgproc.cvtColor(matSource, dest, code);
            return dest;
        }

        public static Mat Resize(this Mat matSource, double fx, double fy, int interpolation = Imgproc.INTER_AREA)
        {
            var matDest = new Mat();
            Imgproc.resize(matSource, matDest, new Size(0, 0), fx, fy, interpolation);
            return matDest;
        }

        public static Mat Resize(this Mat matSource, int width, int height, bool overwrite, int interpolation = Imgproc.INTER_AREA)
        {
            var matDest = new Mat();
            Imgproc.resize(matSource, matDest, new Size(width, height), 1.0, 1.0, interpolation);
            if (overwrite)
            {
                matSource.Dispose();
            }
            return matDest;
        }

        public static void SaveJPEG(this Mat matSource, string filename, bool isRevealInFinder = false)
        {
#if UNITY_EDITOR
            Imgcodecs.imwrite(filename, matSource);
            if (isRevealInFinder)
            {
                UnityEditor.EditorUtility.RevealInFinder(Application.temporaryCachePath);
            }
#endif
        }




        public static Mat Copy(this Mat matSource)
        {
            var matDest = new Mat();
            matSource.copyTo(matDest);
            return matDest;
        }
    }
}