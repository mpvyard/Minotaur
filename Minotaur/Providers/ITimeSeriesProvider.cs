﻿using System;
using System.Collections.Generic;
using Minotaur.Cursors;

namespace Minotaur.Providers
{
    public interface ITimeSeriesProvider
    {
        ICursor GetCursor(string symbol, DateTime start, DateTime? end = null, int[] fields = null);
    }

    
    public class FileMetaData
    {
        public string Symbol { get; set; }

        public string Column { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        // Todo: The file path creation has to be owmn by the data collector
        public string FilePath { get; set; }
        
        //public string GetFilePath(string folder)
        //    => Path.Combine(folder, Start.Year.ToString(), Symbol, $"{Symbol}_{Column}_{Start:yyyy-MM-dd_HH:mm:ss}.min");
    }
}