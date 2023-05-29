using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Data = Google.Apis.Sheets.v4.Data;

namespace SheetsSample
{
    public class SheetsExample
    {
        public static SheetsExample Instance;
        public void testReadGGSheet()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "1kNA_nUVWuntgS1_Ghm780Ud0Yu7NWHSn49VWnsJ4FH4";
            String range = "language_config_1!A2:E";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            Console.Read();
        }
        public void Main()
        {
            testReadGGSheet();
            //SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
            //{
            //    HttpClientInitializer = GetCredential(),
            //    ApplicationName = "Google-SheetsSample/0.1",
            //});

            //// The spreadsheet to request.
            //string spreadsheetId = "my-spreadsheet-id";  // TODO: Update placeholder value.

            //// The DataFilters used to select which ranges to retrieve from
            //// the spreadsheet.
            //List<Data.DataFilter> dataFilters = new List<Data.DataFilter>();  // TODO: Update placeholder value.

            //// True if grid data should be returned.
            //// This parameter is ignored if a field mask was set in the request.
            //bool includeGridData = false;  // TODO: Update placeholder value.

            //// TODO: Assign values to desired properties of `requestBody`:
            //Data.GetSpreadsheetByDataFilterRequest requestBody = new Data.GetSpreadsheetByDataFilterRequest();
            //requestBody.DataFilters = dataFilters;
            //requestBody.IncludeGridData = includeGridData;

            //SpreadsheetsResource.GetByDataFilterRequest request = sheetsService.Spreadsheets.GetByDataFilter(requestBody, spreadsheetId);

            //// To execute asynchronously in an async method, replace `request.Execute()` as shown:
            //Data.Spreadsheet response = request.Execute();
            //// Data.Spreadsheet response = await request.ExecuteAsync();

            //// TODO: Change code below to process the `response` object:
            //Console.WriteLine(JsonConvert.SerializeObject(response));
        }

        public static UserCredential GetCredential()
        {
            // TODO: Change placeholder below to generate authentication credentials. See:
            // https://developers.google.com/sheets/quickstart/dotnet#step_3_set_up_the_sample
            //
            // Authorize using one of the following scopes:
            //     "https://www.googleapis.com/auth/drive"
            //     "https://www.googleapis.com/auth/drive.file"
            //     "https://www.googleapis.com/auth/spreadsheets"
            return null;
        }
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        
    }
}

