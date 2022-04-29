//-----------------------------------------------------------------------
// <copyright file="UIPaintingView.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using OpenTK.Platform.iPhoneOS;
using OpenTK.Graphics.ES11;
using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using OpenGLES;
using System.Runtime.InteropServices;
using CoreGraphics;
using System.Collections.Generic;
using System.Text;


namespace Hydrogen.iOS {


    // simpler way to do this -- http://stackoverflow.com/questions/5461819/draw-into-uiimage
    public class UIPaintingView : iPhoneOSGameView {
        public const int BrushPixelStep = 3;
        public bool Enabled;
        uint drawingTexture;
        bool firstTouch;
        CGPoint Location;
        CGPoint PreviousLocation;
        UIColor BrushColor;
        UIColor BackgroundColor;

        public List <List<CGPoint>> signaturelines = new List <List<CGPoint>> ();
        public StringBuilder pointlist;

        [Foundation.Export("layerClass")]
        public static Class LayerClass () {
            return iPhoneOSGameView.GetLayerClass ();
        }
		
        public UIPaintingView (CGRect frame) : base (frame)	{
            Enabled = true;
            LayerRetainsBacking = true;
            LayerColorFormat    = EAGLColorFormat.RGBA8;
            ContextRenderingApi = EAGLRenderingAPI.OpenGLES1;
            CreateFrameBuffer();
            MakeCurrent();
            GL.Disable (All.Dither);
            GL.MatrixMode (All.Projection);
            GL.Ortho (0, (float)frame.Width, 0, (float)frame.Height, -1, 1);
            GL.MatrixMode (All.Modelview);
            GL.Enable (All.Texture2D);
            GL.EnableClientState (All.VertexArray);
            GL.Enable (All.PointSpriteOes);
            GL.TexEnv (All.PointSpriteOes, All.CoordReplaceOes, (float) All.True);
            GL.PointSize (4.0f); // was 2.0f //  width / BrushScale);  <-- this seems to be the pen width, changing from 2 to 4 stopped
            BrushColor = UIColor.Blue;
            BackgroundColor = UIColor.White;
            // the gaps happening in the line - but does it really look smooth ?
            pointlist = new StringBuilder(100);
            Erase ();
        }

        protected override void Dispose (bool disposing) {
            base.Dispose (disposing);
            GL.DeleteTextures (1, ref drawingTexture);
        }

        public void Erase () {
            // this defines the background colour of the rectangle that the signature
            // is drawn on	(r, g, b and alpha) but alpha doesn't seem to do anythihg
            nfloat r,g,b,a;
            BackgroundColor.GetRGBA(out r, out g, out b, out a);
            GL.ClearColor((float)r, (float)g, (float)b, (float)a);
            GL.Clear ((uint) All.ColorBufferBit);
            SwapBuffers();
            pointlist.Length = 0;
        }
		
        nfloat[] vertexBuffer;
        int vertexMax = 64;
        nfloat lastx = 0, lasty = 0;

        // this is a bit misleading because the start parameter isn't actually used - the start co-ordinates
        // are taken from the lastx, lasty vars
        private void RenderLineFromPoint (CGPoint start, CGPoint end) {


            int vertexCount = 0;
            if (vertexBuffer == null) {
                vertexBuffer = new nfloat [vertexMax * 2];
            }
			
            var count = Math.Max (Math.Ceiling (Math.Sqrt ((end.X - lastx) * (end.X - lastx) + (end.Y - lasty) * (end.Y - lasty)) / BrushPixelStep),1);
            for (int i = 0; i < count; ++i, ++vertexCount) {
                if (vertexCount == vertexMax) {
                    vertexMax *= 2;
                    Array.Resize (ref vertexBuffer, vertexMax * 2);
                }
                vertexBuffer [2 * vertexCount + 0] = lastx + (end.X - lastx) * (float) i / (float) count;
                vertexBuffer [2 * vertexCount + 1] = lasty + (end.Y - lasty) * (float) i / (float) count;
            }
			
            // needed for both
            GL.VertexPointer (2, All.Float, 0, vertexBuffer);
            GL.DrawArrays (All.Points, 0, vertexCount);
            SwapBuffers ();
            lastx = end.X;
            lasty = end.Y;
        }
		
        public override void TouchesBegan (NSSet touches, UIEvent e) {
            // Set the color using OpenGL
            // this sets the pen colour
            nfloat red,green,blue,alpha;
            BrushColor.GetRGBA(out red, out green, out blue, out alpha);
            GL.Color4((float)red, (float)green, (float)blue, (float)alpha); 

            var bounds = Bounds;
            var touch = (UITouch) e.TouchesForView (this).AnyObject;
            firstTouch = true;
            Location = touch.LocationInView (this);
            Location.Y = bounds.Height - Location.Y;
            if (pointlist.Length != 0)
                pointlist.Append("|,");
            pointlist.Append(Location.X.ToString("N0") + "," + Location.Y.ToString("N0") + ",");
            lastx = Location.X;
            lasty = Location.Y;
        }
		
        public override void TouchesMoved (NSSet touches, UIEvent e) {
            var bounds = Bounds;
            var touch = (UITouch) e.TouchesForView (this).AnyObject;
            if (firstTouch) {
                firstTouch = false;
                PreviousLocation = touch.PreviousLocationInView (this);
                PreviousLocation.Y = bounds.Height - PreviousLocation.Y;
            } else {
                Location = touch.LocationInView (this);
                Location.Y = bounds.Height - Location.Y;
                PreviousLocation = touch.PreviousLocationInView (this);
                PreviousLocation.Y = bounds.Height - PreviousLocation.Y;
            }
			
            if (Enabled)
                RenderLineFromPoint (PreviousLocation, Location);
			
            pointlist.Append(Location.X.ToString("N0") + "," + Location.Y.ToString("N0") + ",");
        }

        public override void TouchesEnded (NSSet touches, UIEvent e) {
            var bounds = Bounds;
            var touch = (UITouch) e.TouchesForView (this).AnyObject;
            if (firstTouch) {
                firstTouch = false;
                PreviousLocation = (CGPoint)(touch.PreviousLocationInView (this));
                PreviousLocation.Y = bounds.Height - PreviousLocation.Y;
                RenderLineFromPoint (PreviousLocation, Location);
            }
        }

        public override void TouchesCancelled (NSSet touches, UIEvent e) {
	
        }



    /*	public void SaveToImage() {
            int x;
            for(x = 0; x < pointlist.Count - 2; ++x) {
                if (pointlist[x + 1].X == 0f)
                    x+=2;

                lastx = pointlist[x].X;
                lasty = pointlist[x].Y;
                RenderLineFromPoint(pointlist[x], pointlist[x + 1]);
            }
        }*/




	
    }
}

