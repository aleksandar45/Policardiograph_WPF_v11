using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using SharpGL.WPF;

namespace Policardiograph_App.ViewModel.OpenGLRender
{
    public class OpenGlDisplay
    {
        IntArray array;
        OpenGLControl openGLControl;

        private bool visible = false;
        public bool Visible {
            get {
                return visible;
            }
            set {
                visible = value;
                if (visible)
                {
                    iterator = 0;
                    openGLControl.Dispatcher.BeginInvoke((Action) (()=>{openGLControl.Visibility = System.Windows.Visibility.Visible;}));  
                }
                else
                {
                    openGLControl.Dispatcher.BeginInvoke((Action)(() => { openGLControl.Visibility = System.Windows.Visibility.Hidden; }));
                }
            }
        }

        private bool fbgaMode = false;
        public bool FBGAMode {
            get {
                return fbgaMode;
            }
            set {
                fbgaMode = value;
            }
        }

        float rotation = 0.0f;
        float r, g, b;
        float dx;
        float scale_factor=1.0f;
        float previous_data_float;
        int iterator=0;
        int max_y = -16777216;
        int min_y = 16777216;
        int max_y_current = 10;
        int min_y_current = -10;
        int previous_y=0;
        int iterator_max = 0;
        int iterator_min = 0;

        public OpenGlDisplay(OpenGLControl openGLControl,bool fbgaMode, float r, float g, float b)
        {
            FBGAMode = fbgaMode;
            this.openGLControl = openGLControl;                  
            this.r = r;
            this.g = g;
            this.b = b;
            OpenGL gl = openGLControl.OpenGL;
            gl.ClearColor(0.9f, 0.9f, 0.9f, 0);
      
        }
       
        public void linkArray(IntArray array) {
            this.array = array;
            this.dx = (float)(4.0 / array.size);
        }
        public void render(){



                if (visible)
                {
                    if (fbgaMode)
                    {
                        //  Get the OpenGL object.
                        OpenGL gl = openGLControl.OpenGL;

                        float x, y;

                        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);                      //  Clear the color and depth buffer.
                        gl.DrawText(0, (int)(0.1428 * gl.RenderContextProvider.Height), 0.0f, 0f, 0f, null, 9f, string.Format("{0:#0.##}", (float)2000.0 / scale_factor));
                        gl.DrawText(0, (int)(0.857 * gl.RenderContextProvider.Height), 0.0f, 0f, 0f, null, 9f, string.Format("{0:#0.##}", (float)60000.0 / scale_factor));
                        gl.DrawText(0, (int)(0.5 * gl.RenderContextProvider.Height), 0.0f, 0f, 0f, null, 9f, string.Format("{0:#0.##}", (float)(2000.0 + 60000.0) / (scale_factor * 2.0)));

                        gl.LoadIdentity();
                        gl.Color(r, g, b);
                        gl.LineWidth(1.5f);
                        gl.Begin(OpenGL.GL_LINE_STRIP);
                        // gl.Vertex(0.0f, 0.0f, 0.0f);
                        // gl.Vertex(4.0f, 1.0f, 0.0f);

                        for (int i = 0; i < 512; i++)
                        {
                            y = ((float)array.intArray[i] - (float) 2000.0) / ((float) 60000.0 - (float) 2000.0);
                            x = (float)(dx * i);
                            gl.Vertex((float)x, (float)y, 0.0f);

                        }

                    }
                    else
                    {
                        //  Get the OpenGL object.
                        OpenGL gl = openGLControl.OpenGL;

                        float x, y;
                        double temp_max = 0.0;
                        double temp_min = 0.0;

                        if (iterator > (array.index + 300))
                        {

                            temp_max = max_y + (max_y - min_y) * 0.1;                       
                            max_y_current = (int)temp_max;
                            temp_min = min_y - (max_y - min_y) * 0.1;                          
                            min_y_current = (int)temp_min;
                            max_y = -16777216;
                            min_y = 16777216;
                        }
                        iterator = array.index;

                        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);                      //  Clear the color and depth buffer.
                        gl.DrawText(0, (int)(0.1428 * gl.RenderContextProvider.Height), 0.0f, 0f, 0f, null, 9f, string.Format("{0:#0.##}", (float)min_y_current / scale_factor));
                        gl.DrawText(0, (int)(0.857 * gl.RenderContextProvider.Height), 0.0f, 0f, 0f, null, 9f, string.Format("{0:#0.##}", (float)max_y_current / scale_factor));
                        gl.DrawText(0, (int)(0.5 * gl.RenderContextProvider.Height), 0.0f, 0f, 0f, null, 9f, string.Format("{0:#0.##}", (float)(min_y_current + max_y_current) / (scale_factor * 2.0)));



                        gl.LoadIdentity();
                        gl.Color(r, g, b);
                        gl.LineWidth(1.5f);
                        gl.Begin(OpenGL.GL_LINE_STRIP);
                        // gl.Vertex(0.0f, 0.0f, 0.0f);
                        // gl.Vertex(4.0f, 1.0f, 0.0f);

                        for (int i = 0; i < iterator; i++)
                        {
                            if (min_y_current != max_y_current)
                            {
                                //previous_data_float = ((float)previous_y - (float)min_y_current) / ((float)max_y_current - (float)min_y_current); // (float)(16777216.0);
                                //y = array.intArray[iterator]  / (float)(7216.0);
                                y = ((float)array.intArray[i] - (float)min_y_current) / ((float)max_y_current - (float)min_y_current);
                                if (y < 0) {
                                    previous_data_float = array.intArray[i];
                                    previous_data_float = 0;
                                }
                            }
                            else
                            {
                                previous_data_float = 0.5f;
                                y = 0.5f;
                            }
                            if (array.intArray[i] > max_y)
                            {
                                max_y = array.intArray[i];
                                iterator_max = iterator;
                            }

                            if (array.intArray[i] < min_y)
                            {
                                min_y = array.intArray[i];
                                iterator_min = iterator;
                            }

                            x = (float)(dx * i);
                            gl.Vertex((float)x, (float)y, 0.0f);
                        }



                        //  Rotate around the Y axis.
                        /*               gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

                                        //  Draw a coloured pyramid.
                                        gl.Begin(OpenGL.GL_TRIANGLES);
                                        gl.Color(1.0f, 0.0f, 0.0f);
                                        gl.Vertex(0.0f, 1.0f, 0.0f);
                                        gl.Color(0.0f, 1.0f, 0.0f);
                                        gl.Vertex(-1.0f, -1.0f, 1.0f);
                                        gl.Color(0.0f, 0.0f, 1.0f);
                                        gl.Vertex(1.0f, -1.0f, 1.0f);
                                        gl.Color(1.0f, 0.0f, 0.0f);
                                        gl.Vertex(0.0f, 1.0f, 0.0f);
                                        gl.Color(0.0f, 0.0f, 1.0f);
                                        gl.Vertex(1.0f, -1.0f, 1.0f);
                                        gl.Color(0.0f, 1.0f, 0.0f);
                                        gl.Vertex(1.0f, -1.0f, -1.0f);
                                        gl.Color(1.0f, 0.0f, 0.0f);
                                        gl.Vertex(0.0f, 1.0f, 0.0f);
                                        gl.Color(0.0f, 1.0f, 0.0f);
                                        gl.Vertex(4.0f, -1.0f, -1.0f);
                                        gl.Color(0.0f, 0.0f, 1.0f);
                                        gl.Vertex(-1.0f, -1.0f, -1.0f);
                                        gl.Color(1.0f, 0.0f, 0.0f);
                                        gl.Vertex(0.0f, 1.0f, 0.0f);
                                        gl.Color(0.0f, 0.0f, 1.0f);
                                        gl.Vertex(-1.0f, -1.0f, -1.0f);
                                        gl.Color(0.0f, 1.0f, 0.0f);
                                        gl.Vertex(-1.0f, -1.0f, 1.0f);
                                        gl.End();

                                        //  Nudge the rotation.
                                        rotation += 3.0f;*/


                        gl.End();
                        gl.Flush();
                        openGLControl.InvalidateArrange();
                    }
                }
           // }
        }
    }
}
