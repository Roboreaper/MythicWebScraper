using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGMythicScraper
{
    class CommandLineOptions
    {
        // Define a class to receive parsed values
      
            [Option('s', "set", Required = true, DefaultValue ="", HelpText = "Set to be parsed")]
            public string Set { get; set; }
                 
            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        
    }
}
