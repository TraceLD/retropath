// See https://aka.ms/new-console-template for more information

using RetroPath.Core;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;
using GraphMolWrap;

var inputConfig = new InputConfiguration(
    @"C:\Users\Lukasz\Documents\projects\retropath-standalone\data\retrorules_rr02_rp2_flat_all.csv",
    @"C:\Users\Lukasz\Documents\projects\rp\RetroPath2.0\tutorial_data\pinocembrin\sink_B.csv",
    @"C:\Users\Lukasz\Documents\projects\rp\RetroPath2.0\tutorial_data\pinocembrin\source.csv",
    1000,
    "cofacts_file",
    1000,
    0,
    1000,
    4
);

//var rulesParser = new RulesParser(inputConfig);
//var parsedRules = rulesParser.Parse(inputConfig.RulesFilePath);

using var compoundParser = new CompoundParser(inputConfig);
var parsedSinks = compoundParser.Parse(inputConfig.SinkFilePath, ChemicalType.Sink);
var parsedSources = compoundParser.Parse(inputConfig.SourceFilePath, ChemicalType.Source);

Console.WriteLine("Goodbye!");
