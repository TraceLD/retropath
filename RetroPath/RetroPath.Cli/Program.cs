// See https://aka.ms/new-console-template for more information

using RetroPath.Core;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;
using GraphMolWrap;

var inputConfig = new InputConfiguration(
    @"C:\Users\Lukasz\Documents\projects\retropath-standalone\data\retrorules_rr02_rp2_flat_all.csv",
    "sink_file",
    "source_fule",
    1000,
    0,
    1000,
    4
);

// var rulesParser = new RulesParser(inputConfig);
//var parsedRules = rulesParser.Parse(inputConfig.RulesFilePath);

using var mol = RWMol.MolFromSmiles("fdsafdsafdsafdsa");

Console.WriteLine("Goodbye!");
