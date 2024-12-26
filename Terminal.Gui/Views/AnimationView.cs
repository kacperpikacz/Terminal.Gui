using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Terminal.Gui {
    public class AnimationView : View {
        private View AnimationContainer;
        private ImageView ImageView;

        private int _speed;
        private string _animationPath;

        public AnimationView (string animationPath, int speed = 100)
        {
            Id = "PrivacyMode";

            AnimationContainer = new View () { X = Pos.Center (), Y = Pos.Center (), Width = Dim.Fill (), Height = Dim.Fill () };

            _speed = speed;
            _animationPath = animationPath;

            Add (AnimationContainer);

            ImageView = new ImageView () {
                Width = Dim.Fill (),
                Height = Dim.Fill (),
            };

            AnimationContainer.Add (ImageView);

            Init ();
        }

        public void Init ()
        {
            SetAnimation ();

            Task.Run (() => {

                while (true) {
                    Application.MainLoop.Invoke (() => {
                        ImageView.NextFrame ();
                        ImageView.SetNeedsDisplay ();
                    });

                    Task.Delay (_speed).Wait ();
                }
            });
        }

        void SetAnimation ()
        {
            var executingAssembly = Assembly.GetEntryAssembly ();
            var r = Array.Find (executingAssembly.GetManifestResourceNames (), r => r.Contains (_animationPath));



            if (r != null) {
                var stream = executingAssembly.GetManifestResourceStream (r);

                if (stream != null) {
                    var memoryStream = new MemoryStream ();

                    stream.CopyTo (memoryStream);

                    var byteArray = memoryStream.ToArray ();


                    ImageView.SetImage (Image.Load<Rgba32> (byteArray));
                }
            }
        }
    }

    public class BitmapToBraille {
        public const int CHAR_WIDTH = 2;
        public const int CHAR_HEIGHT = 4;

        const string CHARS = " ⠁⠂⠃⠄⠅⠆⠇⡀⡁⡂⡃⡄⡅⡆⡇⠈⠉⠊⠋⠌⠍⠎⠏⡈⡉⡊⡋⡌⡍⡎⡏⠐⠑⠒⠓⠔⠕⠖⠗⡐⡑⡒⡓⡔⡕⡖⡗⠘⠙⠚⠛⠜⠝⠞⠟⡘⡙⡚⡛⡜⡝⡞⡟⠠⠡⠢⠣⠤⠥⠦⠧⡠⡡⡢⡣⡤⡥⡦⡧⠨⠩⠪⠫⠬⠭⠮⠯⡨⡩⡪⡫⡬⡭⡮⡯⠰⠱⠲⠳⠴⠵⠶⠷⡰⡱⡲⡳⡴⡵⡶⡷⠸⠹⠺⠻⠼⠽⠾⠿⡸⡹⡺⡻⡼⡽⡾⡿⢀⢁⢂⢃⢄⢅⢆⢇⣀⣁⣂⣃⣄⣅⣆⣇⢈⢉⢊⢋⢌⢍⢎⢏⣈⣉⣊⣋⣌⣍⣎⣏⢐⢑⢒⢓⢔⢕⢖⢗⣐⣑⣒⣓⣔⣕⣖⣗⢘⢙⢚⢛⢜⢝⢞⢟⣘⣙⣚⣛⣜⣝⣞⣟⢠⢡⢢⢣⢤⢥⢦⢧⣠⣡⣢⣣⣤⣥⣦⣧⢨⢩⢪⢫⢬⢭⢮⢯⣨⣩⣪⣫⣬⣭⣮⣯⢰⢱⢲⢳⢴⢵⢶⢷⣰⣱⣲⣳⣴⣵⣶⣷⢸⢹⢺⢻⢼⢽⢾⢿⣸⣹⣺⣻⣼⣽⣾⣿";

        public int WidthPixels { get; }
        public int HeightPixels { get; }

        public Func<int, int, bool> PixelIsLit { get; }

        public BitmapToBraille (int widthPixels, int heightPixels, Func<int, int, bool> pixelIsLit)
        {
            WidthPixels = widthPixels;
            HeightPixels = heightPixels;
            PixelIsLit = pixelIsLit;
        }

        public string GenerateImage ()
        {
            int imageHeightChars = (int)Math.Ceiling ((double)HeightPixels / CHAR_HEIGHT);
            int imageWidthChars = (int)Math.Ceiling ((double)WidthPixels / CHAR_WIDTH);

            var result = new StringBuilder ();

            for (int y = 0; y < imageHeightChars; y++) {

                for (int x = 0; x < imageWidthChars; x++) {
                    int baseX = x * CHAR_WIDTH;
                    int baseY = y * CHAR_HEIGHT;

                    int charIndex = 0;
                    int value = 1;

                    for (int charX = 0; charX < CHAR_WIDTH; charX++) {
                        for (int charY = 0; charY < CHAR_HEIGHT; charY++) {
                            int bitmapX = baseX + charX;
                            int bitmapY = baseY + charY;
                            bool pixelExists = bitmapX < WidthPixels && bitmapY < HeightPixels;

                            if (pixelExists && PixelIsLit (bitmapX, bitmapY)) {
                                charIndex += value;
                            }
                            value *= 2;
                        }
                    }

                    result.Append (CHARS [charIndex]);
                }
                result.Append ('\n');
            }
            return result.ToString ().TrimEnd ();
        }
    }

    public class ImageView : View {
        private int frameCount;
        private int currentFrame = 0;

        private Image<Rgba32> [] fullResImages = Array.Empty<Image<Rgba32>> ();
        private Image<Rgba32> [] matchSizes = Array.Empty<Image<Rgba32>> ();
        private string [] brailleCache = Array.Empty<string> ();

        Rect oldSize = Rect.Empty;

        public void SetImage (Image<Rgba32> image)
        {
            frameCount = image.Frames.Count;

            fullResImages = new Image<Rgba32> [frameCount];
            matchSizes = new Image<Rgba32> [frameCount];
            brailleCache = new string [frameCount];

            for (int i = 0; i < frameCount - 1; i++) {
                fullResImages [i] = image.Frames.ExportFrame (0);
            }
            fullResImages [frameCount - 1] = image;

            this.SetNeedsDisplay ();
        }
        public void NextFrame ()
        {
            currentFrame = (currentFrame + 1) % frameCount;
        }

        public override void OnDrawContent (Rect contentArea)
        {
            base.OnDrawContent (contentArea);

            if (oldSize != Bounds) {
                // Invalidate cached images now size has changed
                matchSizes = new Image<Rgba32> [frameCount];
                brailleCache = new string [frameCount];
                oldSize = Bounds;
            }

            var imgScaled = matchSizes [currentFrame];
            var braille = brailleCache [currentFrame];

            if (imgScaled == null) {
                var imgFull = fullResImages [currentFrame];

                // keep aspect ratio
                var newSize = Math.Min (Bounds.Width, Bounds.Height);

                // generate one
                var a = imgFull.Clone ();
                a.Mutate (x => x.Resize (
                         newSize * BitmapToBraille.CHAR_HEIGHT,
                         newSize * BitmapToBraille.CHAR_HEIGHT));

                matchSizes [currentFrame] = a;

            }

            if (braille == null) {
                brailleCache [currentFrame] = braille = GetBraille (matchSizes [currentFrame]);
            }

            var lines = braille.Split ('\n');

            for (int y = 0; y < lines.Length; y++) {
                var line = lines [y];
                for (int x = 0; x < line.Length; x++) {
                    AddRune (x, y, (System.Rune)line [x]);
                }
            }
        }

        private string GetBraille (Image<Rgba32> img)
        {
            var braille = new BitmapToBraille (
                img.Width,
                img.Height,
                (x, y) => IsLit (img, x, y));

            return braille.GenerateImage ();
        }

        private bool IsLit (Image<Rgba32> img, int x, int y)
        {
            var rgb = img [x, y];
            return rgb.R + rgb.G + rgb.B > 50;
        }
    }
}