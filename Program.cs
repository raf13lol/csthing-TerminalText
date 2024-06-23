using TerminalText;

// little example
class Program
{
    static void Main(string[] args)
    {
        Terminal terminal = new Terminal(24);
        // random garble just for showcase
        terminal.AddLine("<b><i><inv><cf-yel>HELP!!!<cf-red>or</cf-yel> <cf-blu>don't <reset>HELP!!!!</reset></cf-red> AND</cf-blu> HELPP!!!</inv></i></b>");
        // the cc24b might not work on all terminals
        terminal.AddLine("<ccgf-23>I can be <ccf-505>any <cc24b-24-74-0>color</cc24b-24-74-0>*</ccf-505>!</ccgf-23>");
        
        terminal.AddLine("<b>You chose: '" + terminal.ChooseFromSelection(["option 1", "option 2", "option 3"]) + "'. </b>");

        string answer = terminal.Question("<inv>Question, are you human? </inv>");
        
        terminal.AddLine("<u><inv>Your answer is: " + answer + "</inv></u>");

        string Go = terminal.Question("See if you can break this, if you can, report it! ");

        terminal.AddLine("Your attempt is: " + Go);
        terminal.AddLine("My attempt of sanitization is: " + TerminalFancyTextParser.ParseTextWithANSICodes(Go, true));

        // read key works really weirdly... do it if you want but ehhh uhhh hmmm
        // terminal.AddLine("<i>key: " + terminal.ReadKey() + "</i>");
        terminal.Render();

        terminal.ReadLine();
    }
}