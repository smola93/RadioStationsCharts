using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.IO;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RadioStationsCharts.Attributes;
using System.Net.Mail;

namespace RadioStationsCharts.Controllers
{
    [Route("scraping")]
    [ApiController]
    [ApiKey]
    public class ScrapingController : ControllerBase
    {
        readonly DatabaseAccess db;
        private readonly IConfiguration Configuration;
        public ScrapingController(IConfiguration config)
        {
            Configuration = config;
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
                HttpContext.Response.StatusCode = 500;
                SendEmailWithScrapingError(ex.Message, "ScrapRmfFmCharts");
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
                HttpContext.Response.StatusCode = 500;
                SendEmailWithScrapingError(ex.Message, "ScrapEskaCharts");
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
                HttpContext.Response.StatusCode = 500;
                SendEmailWithScrapingError(ex.Message, "ScrapRadioZetCharts");
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
                HttpContext.Response.StatusCode = 500;
                SendEmailWithScrapingError(ex.Message, "ScrapVoxFmCharts");
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
                HttpContext.Response.StatusCode = 500;
                SendEmailWithScrapingError(ex.Message, "ScrapPolskieRadio1Charts");
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
                HttpContext.Response.StatusCode = 500;
                SendEmailWithScrapingError(ex.Message, "ScrapTrojkaCharts");
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
                HtmlNodeCollection artistDetails = node.SelectNodes(".//a");
                HtmlNodeCollection titleDetails = node.SelectNodes(".//text()");

                DataRow row = dt.NewRow();
                row["Number"] = number - 1;
                row["Artist"] = artistDetails[0].InnerText;
                row["Title"] = titleDetails[titleDetails.Count - 1].InnerText;
                if (number != 1)
                {
                    dt.Rows.Add(row);
                }
                if (number == 21)
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
        private void SendEmailWithScrapingError(string exceptionMessage, string methodName)
        {
            string emailPass = Configuration.GetSection("MailPass").Value;
            string message = $"Error occurred in method: {methodName}: \n\n{exceptionMessage}";
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("radiostationscharts@gmail.com", emailPass),
                    EnableSsl = true,
                };

                smtpClient.Send("radiostationscharts@gmail.com", Configuration.GetSection("MyEmail").Value, $"Error occured in {methodName} method!", message);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
        }
    }
}
