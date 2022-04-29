using OpenTK;
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace CG_lab5
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        /*public class Game : GameWindow {
            public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }

            void override On
        }*/


        public class Game
        {
            public GameWindow window;

            public Game(GameWindow windowInput)
            {
                this.window = windowInput;

                window.Load += windowLoad;
                window.RenderFrame += windowRender;
                window.UpdateFrame += windowUpdate;
                window.Closing += windowClose;
            }

            private void windowLoad(object sender, EventArgs e)
            {
                GL.ClearColor(Color.Red);

            }

            private void windowClose(object sender, CancelEventArgs e)
            {
                throw new NotImplementedException();
            }

            private void windowUpdate(object sender, FrameEventArgs e)
            {
                Console.WriteLine("Update");
            }

            private void windowRender(object sender, FrameEventArgs e)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.Flush();
                window.SwapBuffers();
            }
        }




            [STAThread]
        static void Main()
        {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());

               /* GameWindow window = new GameWindow(800, 600);
                Game game = new Game(window);

            window.Run();*/
        }
    }
}
