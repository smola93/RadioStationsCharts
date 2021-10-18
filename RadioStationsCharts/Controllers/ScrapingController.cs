using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.IO;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RadioStationsCharts.Controllers
{
    [Route("scraping")]
    [ApiController]
    public class ScrapingController : ControllerBase
    {
        readonly DatabaseAccess db;
        public ScrapingController(IConfiguration config)
        {
            db = new DatabaseAccess(config);
        }

        [HttpGet]
        [Route("scrap-rmf")]
        public string ScrapRmfFmCharts()
        {
            try
            {
                DataTable charts = new DataTable();
                charts.Columns.Add("Number");
                charts.Columns.Add("Artist");
                charts.Columns.Add("Title");

                string url = "https://www.rmf.fm/au/poplista/";
                charts = ParseRmfHtmlToDataTable(url, charts);

                db.ExecDatatableProcedure("UpdateRmfCharts", charts);

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        
        [HttpGet]
        [Route("scrap-eska")]
        public string ScrapEskaCharts()
        {
            try
            {
                DataTable charts = new DataTable();
                charts.Columns.Add("Number");
                charts.Columns.Add("Artist");
                charts.Columns.Add("Title");

                string url = "https://www.eska.pl/goraca20/";
                charts = ParseEskaAndVoxHtmlToDataTable(url, charts);

                db.ExecDatatableProcedure("UpdateEskaCharts", charts);

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [HttpGet]
        [Route("scrap-radio-zet")]
        public string ScrapRadioZetCharts()
        {
            try
            {
                DataTable charts = new DataTable();
                charts.Columns.Add("Number");
                charts.Columns.Add("Artist");
                charts.Columns.Add("Title");

                string url = "https://www.radiozet.pl/Radio/Lista-przebojow";
                charts = ParseRadioZetHtmlToDataTable(url, charts);

                db.ExecDatatableProcedure("UpdateRadioZetCharts", charts);

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        [HttpGet]
        [Route("scrap-vox-fm")]
        public string ScrapVoxFmCharts()
        {
            try
            {
                DataTable charts = new DataTable();
                charts.Columns.Add("Number");
                charts.Columns.Add("Artist");
                charts.Columns.Add("Title");

                string url = "https://www.voxfm.pl/bestlista/";
                charts = ParseEskaAndVoxHtmlToDataTable(url, charts);

                db.ExecDatatableProcedure("UpdateVoxFmCharts", charts);

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        [HttpGet]
        [Route("scrap-polskie-radio-1")]
        public string ScrapPolskieRadio1Charts()
        {
            try
            {
                DataTable charts = new DataTable();
                charts.Columns.Add("Number");
                charts.Columns.Add("Artist");
                charts.Columns.Add("Title");

                string url = "http://ppr1.polskieradio.pl/";
                charts = ParsePolskieRadio1HtmlToDataTable(url, charts);

                db.ExecDatatableProcedure("UpdatePolskieRadio1Charts", charts);

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        [HttpGet]
        [Route("scrap-trojka")]
        public string ScrapTrojkaCharts()
        {
            try
            {
                DataTable charts = new DataTable();
                charts.Columns.Add("Number");
                charts.Columns.Add("Artist");
                charts.Columns.Add("Title");

                string url = "https://lp3.polskieradio.pl/";
                charts = ParseTrojkaHtmlToDataTable(url, charts);

                db.ExecDatatableProcedure("UpdateTrojkaCharts", charts);

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        private DataTable ParseRmfHtmlToDataTable(string url, DataTable dt)
        {
            int number = 0;

            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData(url));
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(ms, Encoding.GetEncoding("ISO-8859-2"));
            var chartsNode = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'poplista-artist-title')]");
            
            foreach (HtmlNode node in chartsNode)
            {
                number++;
                HtmlNodeCollection chartsDetails = node.SelectNodes(".//a");

                DataRow row = dt.NewRow();
                row["Number"] = number;
                row["Artist"] = chartsDetails[0].InnerText;
                row["Title"] = chartsDetails[1].InnerText;
                dt.Rows.Add(row);
                if (number == 20)
                {
                    break;
                }
            }

            return dt;

        }
        private DataTable ParseEskaAndVoxHtmlToDataTable(string url, DataTable dt)
        {
            int number = 0;

            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData(url));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(ms, Encoding.UTF8);
            var chartsNode = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'single-hit__info')]");

            foreach (HtmlNode node in chartsNode)
            {
                HtmlNodeCollection artistDetails = node.SelectNodes(".//li");
                HtmlNodeCollection titleDetails = node.SelectNodes(".//a");
                List<string> artists = new List<string>();
     
                if (titleDetails != null && artistDetails != null)
                {
                    number++;
                    foreach (var item in artistDetails)
                    {
                        artists.Add(item.InnerText.Trim());
                    }
                    DataRow row = dt.NewRow();
                    row["Number"] = number;
                    string artist = string.Join(", ", artists);
                    row["Artist"] = HttpUtility.HtmlDecode(artist);
                    row["Title"] = HttpUtility.HtmlDecode(titleDetails[0].InnerText);
                    dt.Rows.Add(row);
                }
                if (number == 20)
                {
                    break;
                }
            }

            return dt;

        }
        private DataTable ParseRadioZetHtmlToDataTable(string url, DataTable dt)
        {
            int number = 0;

            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData(url));

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(ms, Encoding.UTF8);
            var artists = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'artist-track')]");
            var titles = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'title-track')]");

            foreach (HtmlNode artist in artists)
            {
                number++;
                DataRow row = dt.NewRow();
                row["Number"] = number;
                row["Artist"] = artist.InnerText.Replace("\n", "");
                row["Title"] = titles[number - 1].InnerText.Replace("\n", "");
                dt.Rows.Add(row);
                if (number == 30)
                {
                    break;
                }
            }

            return dt;

        }
        private DataTable ParsePolskieRadio1HtmlToDataTable(string url, DataTable dt)
        {
            int number = 0;

            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData(url));

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(ms, Encoding.UTF8);
            var artists = htmlDoc.DocumentNode.SelectNodes("//span[contains(@class, 'artist')]");
            var titles = htmlDoc.DocumentNode.SelectNodes("//span[contains(@class, 'title')]");

            foreach (HtmlNode artist in artists)
            {
                number++;
                DataRow row = dt.NewRow();
                row["Number"] = number;
                row["Artist"] = artist.InnerText.Replace("\n", "");
                row["Title"] = titles[number - 1].InnerText.Replace("\n", "");
                dt.Rows.Add(row);
            }

            return dt;

        }
        private DataTable ParseTrojkaHtmlToDataTable(string url, DataTable dt)
        {
            int number = 0;

            var options = new ChromeOptions()
            {
                BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };
            options.AddArguments(new List<string>() { "headless", "disable-gpu" });
            var browser = new ChromeDriver(options);
            
            try
            {
                browser.Navigate().GoToUrl(url);

                IWebElement element = browser.FindElement(By.ClassName("select-button"));
                IJavaScriptExecutor js = browser;
                js.ExecuteScript("var evt = document.createEvent('MouseEvents');" + 
                    "evt.initMouseEvent('click',true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0,null);" 
                    + "arguments[0].dispatchEvent(evt);", element);

                var firstPositionArtist = browser.FindElementsByClassName("list-first-element__info-artist");
                var firstPositionTitle = browser.FindElementsByClassName("list-first-element__info-title");
                var artists = browser.FindElementsByClassName("list-rest__info-artist");
                var titles = browser.FindElementsByClassName("list-rest__info-title");

                number++;
                DataRow firstRow = dt.NewRow();
                firstRow["Number"] = number;
                firstRow["Artist"] = firstPositionArtist[0].Text;
                firstRow["Title"] = firstPositionTitle[0].Text;
                dt.Rows.Add(firstRow);

                foreach (var artist in artists)
                {
                    number++;
                    DataRow row = dt.NewRow();
                    row["Number"] = number;
                    row["Artist"] = artist.Text;
                    row["Title"] = titles[number - 2].Text;
                    dt.Rows.Add(row);
                    if (number == 30)
                    {
                        break;
                    }
                }
                
                browser.Quit();
                
                return dt;
            }
            catch (Exception)
            {
                browser.Quit();
                throw;
            }

        }
        //Do usuniecia?
        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetAsync(fullUrl);
            response.Content.Headers.ContentType.CharSet = @"ISO-8859-1";
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
