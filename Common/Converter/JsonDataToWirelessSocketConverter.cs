﻿using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToWirelessSocketConverter : IJsonDataConverter<WirelessSocketDto>
    {
        private const string TAG = "JsonDataToWirelessSocketConverter";
        private static string _searchParameter = "{\"Data\":";

        private readonly Logger _logger;

        public JsonDataToWirelessSocketConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<WirelessSocketDto> GetList(string[] jsonStringArray)
        {
            if (StringHelper.StringsAreEqual(jsonStringArray))
            {
                return parseStringToList(jsonStringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(jsonStringArray, _searchParameter);
                return parseStringToList(usedEntry);
            }
        }

        public IList<WirelessSocketDto> GetList(string jsonString)
        {
            _logger.Debug(string.Format("GetList with jsonString {0}", jsonString));

            return parseStringToList(jsonString);
        }

        private IList<WirelessSocketDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<WirelessSocketDto> wirelessSocketList = new List<WirelessSocketDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                int id = 0;
                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken wirelessSocketJsonData = child["WirelessSocket"];

                    string name = wirelessSocketJsonData["Name"].ToString();
                    string area = wirelessSocketJsonData["Area"].ToString();
                    string code = wirelessSocketJsonData["Code"].ToString();

                    bool isActivated = wirelessSocketJsonData["State"].ToString() == "1";

                    JToken lastTriggerJsonData = wirelessSocketJsonData["LastTrigger"];

                    int year = int.Parse(lastTriggerJsonData["Year"].ToString());
                    int month = int.Parse(lastTriggerJsonData["Month"].ToString());
                    int day = int.Parse(lastTriggerJsonData["Day"].ToString());
                    int hour = int.Parse(lastTriggerJsonData["Minute"].ToString());
                    int minute = int.Parse(lastTriggerJsonData["Hour"].ToString());

                    if (year == -1 || month == -1 || day == -1 || hour == -1 || minute == -1)
                    {
                        year = 1970;
                        month = 1;
                        day = 1;
                        hour = 0;
                        minute = 0;
                    }

                    DateTime lastTriggerDate = new DateTime(year, month, day, hour, minute, 0);

                    string lastTriggerUser = lastTriggerJsonData["UserName"].ToString();

                    WirelessSocketDto newMenu = new WirelessSocketDto(id, name, area, code, isActivated, lastTriggerDate, lastTriggerUser);
                    wirelessSocketList.Add(newMenu);

                    id++;
                }

                return wirelessSocketList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<WirelessSocketDto>();
        }
    }
}
