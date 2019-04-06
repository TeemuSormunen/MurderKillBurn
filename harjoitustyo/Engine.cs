using System;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace harjoitustyo
{
    internal class Engine
    {
        private DispatcherTimer timer = new DispatcherTimer();

        public void GameOver()
        {
            timer.Stop();
            //MessageBox.Show("Your score: " + score);
            //this.Close();
            GameOverShow();
        }

        private void GameOverShow()
        {
            //   txtMessage.Text = "Your score: " + score + "\npress Esc to quit";
            //  paintCanvas.Children.Add(txtMessage);
            //animaatio joka siirtää kanvaasin
            var trs = new TranslateTransform();
            var anim = new DoubleAnimation(0, 620, TimeSpan.FromSeconds(15));
            trs.BeginAnimation(TranslateTransform.XProperty, anim);
            trs.BeginAnimation(TranslateTransform.YProperty, anim);
            //this.RenderTransform = trs;
        }
    }
}