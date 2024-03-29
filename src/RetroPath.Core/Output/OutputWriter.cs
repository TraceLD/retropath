﻿using System.Globalization;
using CsvHelper;

namespace RetroPath.Core.Output;

public class CsvOutputWriter<T>
{
    private readonly string _filePath;
    private readonly IEnumerable<T> _resultsToWrite;

    public CsvOutputWriter(string outputDirPath, string fileName, IEnumerable<T> resultsToWrite)
    {
        _filePath = Path.Combine(outputDirPath, fileName);
        _resultsToWrite = resultsToWrite;
    }

    public void Write()
    {
        using var writer = new StreamWriter(_filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteRecords(_resultsToWrite);
    }

    public async Task WriteAsync()
    {
        await using var writer = new StreamWriter(_filePath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        await csv.WriteRecordsAsync(_resultsToWrite);
    }
}