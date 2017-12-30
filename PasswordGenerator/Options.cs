using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordGenerator
{
    class Options
    {
        [Option('l', "length", Required = true,
          HelpText = "Password length.")]
        public int PasswordLength { get; set; }

        [Option('n', "num", Required = false, DefaultValue = 1,
          HelpText = "Number of Passwords.")]
        public int NumPassword { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(
                this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
