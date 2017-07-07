﻿using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Common.Dto.TemperatureDto;

namespace Common.Converter
{
    public class JsonDataToTemperatureConverter
    {
        private const string TAG = "JsonDataToTemperatureConverter";
        private static string _searchParameter = "{temperature:";

        private static Logger _logger;

        public JsonDataToTemperatureConverter()
        {
            _logger = new Logger(TAG);
        }

        public static IList<TemperatureDto> GetList(string[] stringArray)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return ParseStringToList(stringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return ParseStringToList(usedEntry);
            }
        }

        private static IList<TemperatureDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<TemperatureDto> list = new List<TemperatureDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        foreach (string entry in entries)
                        {
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            TemperatureDto newValue = ParseStringToValue(data);
                            if (newValue != null)
                            {
                                list.Add(newValue);
                            }
                        }

                        return list;
                    }
                }
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return null;
        }

        private static TemperatureDto ParseStringToValue(string[] data)
        {
            if (data.Length == 4)
            {
                if (data[0].Contains("{value:") && data[1].Contains("{area:") && data[2].Contains("{sensorPath:")
                        && data[3].Contains("{graphPath:"))
                {

                    string temperatureString = data[0].Replace("{value:", "").Replace("};", "");
                    double temperature = -1;
                    bool parseSuccessTemperature = double.TryParse(temperatureString, out temperature);
                    if (!parseSuccessTemperature)
                    {
                        _logger.Warning("Failed to parse temperature from data!");
                    }

                    string area = data[1].Replace("{area:", "").Replace("};", "");
                    string sensorPath = data[2].Replace("{sensorPath:", "").Replace("};", "");
                    string graphPath = data[3].Replace("{graphPath:", "").Replace("};", "");

                    DateTime lastUpdate = DateTime.Now;

                    return new TemperatureDto(temperature, area, lastUpdate, sensorPath, TemperatureType.RASPBERRY, graphPath);
                }
                else
                {
                    _logger.Error("data contains invalid entries!");
                }
            }
            else
            {
                _logger.Error(string.Format("Data has invalid length {0}", data.Length));
            }

            _logger.Error(string.Format("{0} has an error!", data));

            return null;
        }
    }
}
