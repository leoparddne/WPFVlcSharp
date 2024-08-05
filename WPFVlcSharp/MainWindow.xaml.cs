using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;

namespace WPFVlcSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LibVLC _libvlc = null;
        bool isMouseVideo = false;


        Timer timer;

        string videoPath = "C:\\Users\\ivesBao\\Desktop\\zhaohuo.mp4";

        public MainWindow()
        {
            InitializeComponent();

            _libvlc = new LibVLC();
            LibVLCSharp.Shared.MediaPlayer player = new LibVLCSharp.Shared.MediaPlayer(_libvlc);

            //videoViewControl.Width = this.Width;
            //videoViewControl.Height = this.Height;
            videoViewControl.MediaPlayer = player;
            //通过设置宽高比为窗体宽高可达到视频铺满全屏的效果
            //player.AspectRatio = this.Width + ":" + this.Height;


            //videoViewControl.MediaPlayer.TimeChanged += MediaPlayer_TimeChanged;

            timer = new Timer(50);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;


        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                UpdatePlayTime();
            });

        }

        private void UpdatePlayTime()
        {
            if (videoViewControl.MediaPlayer == null)
            {
                timer.Stop();
                return;
            }
            ////Trace.WriteLine(videoViewControl.MediaPlayer.AudioTrack);
            //if (!videoViewControl.MediaPlayer.IsPlaying &&
            //    videoViewControl.MediaPlayer.VideoTrack == -1 &&
            //    videoViewControl.MediaPlayer.AudioTrack == -1)
            //{
            //    //获取播放位置是否已经结束,如果结束了讲进度条设置为最大值
            //    //if (videoViewControl.MediaPlayer.Time >= videoViewControl.MediaPlayer.Length && videoViewControl.MediaPlayer.Time != 0 && videoViewControl.MediaPlayer.Length != 0)
            //    //{
            //    Trace.WriteLine($"播放结束-{videoViewControl.MediaPlayer.Time}-{videoViewControl.MediaPlayer.Length}");
            //    sliderProgress.Value = sliderProgress.Maximum;
            //    PauseVideo();
            //    timer.Stop();
            //    return;
            //    //}
            //}

            //if (videoViewControl.MediaPlayer.State == LibVLCSharp.Shared.MediaPlayerState.Ended)
            if (!videoViewControl.MediaPlayer.IsPlaying &&
                videoViewControl.MediaPlayer.VideoTrack == -1 &&
                videoViewControl.MediaPlayer.AudioTrack == -1)
            {
                if (videoViewControl.MediaPlayer.State == VLCState.Ended)
                {
                    Trace.WriteLine($"播放结束-{videoViewControl.MediaPlayer.Time}-{videoViewControl.MediaPlayer.Length}");
                    sliderProgress.Value = sliderProgress.Maximum;
                    PauseVideo();
                    timer.Stop();
                    return;
                }
            }


            //获取当前播放时间和总时间
            var time = (double)videoViewControl.MediaPlayer.Time / 1000;
            var length = (double)videoViewControl.MediaPlayer.Length / 1000;
            var timeSpan = new TimeSpan(0, 0, (int)time);
            var lengthSpan = new TimeSpan(0, 0, (int)length);

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            txtDuringTime.Text = timeSpan.ToString(@"hh\:mm\:ss");
            txtTotalTime.Text = lengthSpan.ToString(@"hh\:mm\:ss");


            ////设置进度条
            if (sliderProgress.Maximum != videoViewControl.MediaPlayer.Length)
            {
                sliderProgress.Maximum = videoViewControl.MediaPlayer.Length;
            }
            sliderProgress.Value = videoViewControl.MediaPlayer.Time;

            //});
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //string url = "rtsp://user:password@192.168.1.120:554/ch1/main/av_stream";
            string url = videoPath;
            using (LibVLCSharp.Shared.Media media = new Media(_libvlc, new Uri(url)))
            {
                var dir = media.Duration;
                videoViewControl.MediaPlayer.Media = media;
                //videoViewControl.MediaPlayer.Play(media);
            }
            PlayVideo();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }

            if (videoViewControl.MediaPlayer.Position == 0 || videoViewControl.MediaPlayer.Position == -1 ||
                videoViewControl.MediaPlayer.AudioTrack == -1 || videoViewControl.MediaPlayer.VideoTrack == -1)
            {
                string url = videoPath;
                using (LibVLCSharp.Shared.Media media = new Media(_libvlc, new Uri(url)))
                {
                    var dir = media.Duration;
                    videoViewControl.MediaPlayer.Media = media;
                    //videoViewControl.MediaPlayer.Play(media);
                }
            }

            PlayVideo();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            PauseVideo();
        }

        public void PlayVideo()
        {
            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }
            btnPause.Visibility = Visibility.Visible;
            btnPlay.Visibility = Visibility.Collapsed;

            //文件名
            txtFileName.Text = "Test" + DateTime.Now.ToString();

            videoViewControl.MediaPlayer.Play();
            timer.Start();
        }

        public void PauseVideo()
        {
            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }

            btnPause.Visibility = Visibility.Collapsed;
            btnPlay.Visibility = Visibility.Visible;
            videoViewControl.MediaPlayer.Pause();
        }

        public void PlayOrPause()
        {
            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }
            if (videoViewControl.MediaPlayer.IsPlaying)
            {
                PauseVideo();
            }
            else
            {
                PlayVideo();
            }
        }

        private void sliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (videoViewControl.MediaPlayer == null || sender == null)
            {
                return;
            }

            if (e.NewValue == videoViewControl.MediaPlayer.Time)
            {
                return;
            }

            //e.NewValue
            //设置进度条
            //videoViewControl.MediaPlayer.Time = (long)sliderProgress.Value;
            videoViewControl.MediaPlayer.Time = (long)e.NewValue;
            //sliderProgress.Maximum = videoViewControl.MediaPlayer.Length;
        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullScream();
        }

        private void ToggleFullScream()
        {
            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }

            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                gridBottom.Visibility = Visibility.Visible;
                videoViewControl.MediaPlayer.Fullscreen = false;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                gridBottom.Visibility = Visibility.Collapsed;
                videoViewControl.MediaPlayer.Fullscreen = true;
                btnVideoCover.Focus();
            }
        }

        /// <summary>
        /// 静音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }

            btnReMute.Visibility = Visibility.Visible;
            btnMute.Visibility = Visibility.Collapsed;
            videoViewControl.MediaPlayer.Mute = true;
        }



        private void btnReMute_Click(object sender, RoutedEventArgs e)
        {

            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }

            btnReMute.Visibility = Visibility.Collapsed;
            btnMute.Visibility = Visibility.Visible;
            videoViewControl.MediaPlayer.Mute = false;
        }


        private void btnVideoCover_Click(object sender, RoutedEventArgs e)
        {
            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }

            //已经播放完毕,重播
            if (videoViewControl.MediaPlayer.State == VLCState.Ended)
            {
                using (LibVLCSharp.Shared.Media media = new Media(_libvlc, new Uri(videoPath)))
                {
                    var dir = media.Duration;
                    videoViewControl.MediaPlayer.Media = media;
                }
            }
            PlayOrPause();
        }

        private void btnVideoCover_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (videoViewControl.MediaPlayer == null)
            {
                return;
            }
            switch (e.Key)
            {
                case System.Windows.Input.Key.Escape:
                    //退出全屏
                    if (videoViewControl.MediaPlayer.Fullscreen)
                    {
                        ToggleFullScream();
                    }
                    break;
                case System.Windows.Input.Key.Space:
                    if (videoViewControl.MediaPlayer.IsPlaying)
                    {
                        PauseVideo();
                    }
                    else
                    {
                        PlayVideo();
                    }
                    break;
                default:
                    break;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //判断是否点击鼠标左键
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (videoViewControl.MediaPlayer != null)
            {
                videoViewControl.MediaPlayer.Dispose();
            }
            _libvlc.Dispose();
            this.Close();
        }

        private void btnVideoCover_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Trace.WriteLine($"{videoViewControl.MediaPlayer}-{videoViewControl.MediaPlayer?.Fullscreen}-{e.LeftButton}-{DateTime.Now}");

            ////全屏模式下禁止拖动
            //if (videoViewControl.MediaPlayer != null&& videoViewControl.MediaPlayer.Fullscreen)
            //{
            //    return;
            //}
            ////判断是否点击鼠标左键
            //if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            //{
            //    Trace.WriteLine("move");
            //    this.DragMove();
            //}
        }

        private void btnVideoCover_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //判断是否点击鼠标左键
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                //如果是全屏或者播放中则不允许拖动
                if (videoViewControl.MediaPlayer != null)
                {
                    //播放中禁止拖动
                    if (videoViewControl.MediaPlayer.AudioTrack != -1 || videoViewControl.MediaPlayer.VideoTrack != -1 ||
                        videoViewControl.MediaPlayer.IsPlaying || videoViewControl.MediaPlayer.Fullscreen)
                    {
                        return;
                    }
                }

                this.DragMove();
            }
        }
    }
}