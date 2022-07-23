// See https://aka.ms/new-console-template for more information

using GraphMolWrap;
using RetroPath.Chem;
using RetroPath.Chem.Utils;
using RetroPath.Core.Chem;
using RetroPath.Core.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var ruleStr = "([#8&v2:1](-[#6&v4:2]1(-[#1&v1:3])-[#6&v4:4](-[#1&v1:5])(-[#1&v1:6])-[#6&v4:7](-[#1&v1:8])(-[#1&v1:9])-[#6&v4:10]2(-[#6&v4:11](-[#1&v1:12])(-[#1&v1:13])-[#1&v1:14])-[#6&v4:15](-[#1&v1:16])(-[#6&v4:17](-[#1&v1:18])(-[#1&v1:19])-[#6&v4:20](-[#1&v1:21])(-[#1&v1:22])-[#6&v4:23]3(-[#6&v4:24](-[#1&v1:25])(-[#1&v1:26])-[#1&v1:27])-[#6&v4:28]4(-[#6&v4:29](-[#1&v1:30])(-[#1&v1:31])-[#1&v1:32])-[#6&v4:33](=[#6&v4:34](-[#1&v1:35])-[#6&v4:36](-[#1&v1:37])(-[#1&v1:38])-[#6&v4:39]-2-3-[#1&v1:40])-[#6&v4:41]2(-[#1&v1:42])-[#6&v4:43](-[#1&v1:44])(-[#1&v1:45])-[#6&v4:46](-[#6&v4:47](-[#1&v1:48])(-[#1&v1:49])-[#1&v1:50])(-[#6&v4:51](-[#1&v1:52])(-[#1&v1:53])-[#1&v1:54])-[#6&v4:55](-[#1&v1:56])(-[#1&v1:57])-[#6&v4:58](-[#8&v2:59]-[#1&v1:60])(-[#1&v1:61])-[#6&v4:62]-2(-[#6&v4:63](-[#1&v1:64])(-[#1&v1:65])-[#1&v1:66])-[#6&v4:67](-[#1&v1:68])(-[#1&v1:69])-[#6&v4:70]-4(-[#1&v1:71])-[#1&v1:72])-[#6&v4:73]-1(-[#6&v4:74](-[#8&v2:75]-[#1&v1:76])(-[#1&v1:77])-[#1&v1:78])-[#6&v4:79](-[#1&v1:80])(-[#1&v1:81])-[#1&v1:82])-[#1&v1:83])>>([#8&v2:1](-[#6&v4:2]1(-[#1&v1:3])-[#6&v4:4](-[#1&v1:5])(-[#1&v1:6])-[#6&v4:7](-[#1&v1:8])(-[#1&v1:9])-[#6&v4:10]2(-[#6&v4:11](-[#1&v1:12])(-[#1&v1:13])-[#1&v1:14])-[#6&v4:15](-[#1&v1:16])(-[#6&v4:17](-[#1&v1:18])(-[#1&v1:19])-[#6&v4:20](-[#1&v1:21])(-[#1&v1:22])-[#6&v4:23]3(-[#6&v4:24](-[#1&v1:25])(-[#1&v1:26])-[#1&v1:27])-[#6&v4:28]4(-[#6&v4:29](-[#1&v1:30])(-[#1&v1:31])-[#1&v1:32])-[#6&v4:33](=[#6&v4:34](-[#1&v1:35])-[#6&v4:36](-[#1&v1:37])(-[#1&v1:38])-[#6&v4:39]-2-3-[#1&v1:40])-[#6&v4:41]2(-[#1&v1:42])-[#6&v4:43](-[#1&v1:44])(-[#1&v1:45])-[#6&v4:46](-[#6&v4:47](-[#1&v1:48])(-[#1&v1:49])-[#1&v1:50])(-[#6&v4:51](-[#1&v1:52])(-[#1&v1:53])-[#1&v1:54])-[#6&v4:55](-[#1&v1:56])(-[#1&v1:57])-[#6&v4:58](-[#1&v1:61])(-[#1&v1:76])-[#6&v4:62]-2(-[#6&v4:63](-[#1&v1:64])(-[#1&v1:65])-[#1&v1:66])-[#6&v4:67](-[#1&v1:68])(-[#1&v1:69])-[#6&v4:70]-4(-[#1&v1:71])-[#1&v1:72])-[#6&v4:73]-1(-[#6&v4:74](-[#1&v1:77])(-[#1&v1:78])-[#1&v1:60])-[#6&v4:79](-[#1&v1:80])(-[#1&v1:81])-[#1&v1:82])-[#1&v1:83].[#8&v2:75]=[#8&v2:59])";
var smartsLeft = ruleStr.Split(">>")[0];
if (smartsLeft.StartsWith("(") && smartsLeft.EndsWith(")"))
{
    smartsLeft = smartsLeft[1..^1];
}

var sourceInchi = "InChI=1S/C48H78O18/c1-21-29(52)31(54)35(58)40(61-21)65-37-32(55)30(53)24(19-49)62-41(37)66-38-34(57)33(56)36(39(59)60)64-42(38)63-28-12-13-45(5)25(46(28,6)20-50)11-14-48(8)26(45)10-9-22-23-17-43(2,3)18-27(51)44(23,4)15-16-47(22,48)7/h9,21,23-38,40-42,49-58H,10-20H2,1-8H3,(H,59,60)";
var standard = new CompoundStandardiser();
var mol = Inchi.InchiToMolSimple(sourceInchi, true, false);
var molS = standard.Standardise(mol!);
var names = new HashSet<string>();

var cc = new ChemicalCompound(names, sourceInchi, "", molS.Mol, false);

Log.Information("Checking...");

using var smartsLeftMol = RWMol.MolFromSmarts(smartsLeft);
// if (cc.Mol!.hasSubstructMatch(smartsLeftMol))
// {
//     Log.Information("Matched");
// }

RDKFuncs.prepareMolForDrawing(smartsLeftMol);

var view = new MolDraw2DSVG(5000, 5000);
view.drawMolecule(smartsLeftMol);
view.finishDrawing();

using var w = new StreamWriter("queryBig.svg");
w.Write(view.getDrawingText());

Log.Information("Done");