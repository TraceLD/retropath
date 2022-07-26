using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Dto;

namespace RetroPath.Tests.TestUtils.Fixtures;

public class PinocembrinResultsFixture
{
    public List<GlobalResult> GlobalResults { get; }

    public PinocembrinResultsFixture()
    {
        var filePath = Path.Combine(Paths.TestDataDir, "pinocembrin_2038_res.csv");
        
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var dtos = csv.GetRecords<GlobalResultDto>();
        var results = dtos.Select(GlobalResult.FromCsvDto).ToList();

        GlobalResults = results;
    }
}