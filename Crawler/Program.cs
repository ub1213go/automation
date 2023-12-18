using AngleSharp;
using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;
using System.Net;
using Tesseract;

namespace Crawler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Practice.TesseractPractice().p1();

            Console.WriteLine();
            Console.WriteLine("按下任意建結束...");
            Console.ReadKey();
        }
    }

    public class Practice
    {
        /// <summary>
        /// 網頁爬蟲
        /// </summary>
        public class AngleSharpPractice
        {
            public async Task p1()
            {
                // AngleSharp 教學
                // https://igouist.github.io/post/2022/06/angle-sharp/
                var config = AngleSharp.Configuration.Default.WithDefaultLoader();

                var browser = BrowsingContext.New(config);

                // 這邊用的型別是 AngleSharp 提供的 AngleSharp.Dom.Url
                //var url = new Url("https://members.china-airlines.com/ffp_b2b/dynastySSO.aspx?lang=zh-tw");

                var url = new Url("https://members.china-airlines.com/ffp_b2b/ValidateCode.aspx");

                // 使用 OpenAsync 來打開網頁抓回內容
                var document = await browser.OpenAsync(url);

                Console.WriteLine(document.Cookie);
            }
        }

        /// <summary>
        /// 輕量本地資料庫
        /// </summary>
        public class SqlitePractice
        {

        }

        /// <summary>
        /// 模擬瀏覽器爬蟲
        /// </summary>
        public class SeleniumPractice
        {
            public async void p1()
            {
                // Selenium 教學
                // https://saucelabs.com/resources/blog/getting-started-with-webdriver-in-c-using-visual-studio
                // Selenium 官方教學
                // https://www.selenium.dev/documentation/webdriver/getting_started/first_script/

                IWebDriver driver = new ChromeDriver();

                //driver.Navigate().GoToUrl("https://www.saucelabs.com");
                driver.Navigate().GoToUrl("https://members.china-airlines.com/ffp_b2b/dynastySSO.aspx?lang=zh-tw");

                IWebElement e = driver.FindElement(By.XPath("(//img)[@id='ContentPlaceHolder1_imgValidate_V3']"));

                //var url = e.GetAttribute("src");
                //Task.Factory.StartNew(() =>
                //{
                //    Thread.Sleep(10000);
                //    driver.Quit();
                //}).Wait();

                // 获取元素的位置和大小
                var location = e.Location;
                var size = e.Size;

                // 若要跨平台參考文章
                // https://blog.darkthread.net/blog/netcore-iamge-processing/
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                using (Bitmap bmp = new Bitmap(new MemoryStream(screenshot.AsByteArray)))
                {
                    Rectangle cropArea = new Rectangle(location, size);
                    Bitmap croppedImage = bmp.Clone(cropArea, bmp.PixelFormat);

                    // 将剪裁出的图像保存为文件或进行其他处理
                    croppedImage.Save(@"D:\Download\1.png");
                }

                //await DownloadImageAsync(@"D:\Download", "1.png", new Uri(url));

                driver.Quit();
            }

            /// <summary>
            /// 下載圖片
            /// </summary>
            private async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri)
            {
                using var httpClient = new HttpClient();

                // Get the file extension
                var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                var fileExtension = Path.GetExtension(uriWithoutQuery);

                // Create file path and ensure directory exists
                //var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
                var path = directoryPath;
                Directory.CreateDirectory(directoryPath);

                // Download the image and write to the file
                var imageBytes = await httpClient.GetByteArrayAsync(uri);
                await File.WriteAllBytesAsync(path, imageBytes);
            }
        }

        /// <summary>
        /// Google 開發的圖片辨識套件，需要訓練資料
        /// </summary>
        public class TesseractPractice
        {
            // Tesseract 圖片轉文字教學
            // https://dev.to/mhamzap10/how-to-use-tesseract-ocr-in-c-9gc
            public void p1()
            {
                //TesseractEnviornment.CustomSearchPath = @"D:\tessdata";
                var ocr = new TesseractEngine(@".\tessdata", "eng", EngineMode.Default);
                ocr.SetVariable("tessedit_char_whitelist", "0123456789");
                var img = Pix.LoadFromFile(@"D:\Download\1.png");
                var res = ocr.Process(img, PageSegMode.Auto).GetText();
            }
        }

        /// <summary>
        /// C# 內建圖片處理 Bitmap
        /// </summary>
        public class BitmapPractice
        {
            public void p1()
            {
                // 圖片處理範例
                var b = new Bitmap(Image.FromFile(@"D:\Download\1.png"));
                var allp = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var ptr = allp.Scan0;
                var size = Math.Abs(allp.Stride) * b.Height;
                var arr = new byte[size];
                unsafe
                {
                    arr[0] = *(byte*)ptr;
                    arr[1] = *((byte*)ptr + 1);
                }

                Console.WriteLine(arr[0]);
                Console.WriteLine(arr[1]);
                Console.ReadKey();
            }
        }
    }
}