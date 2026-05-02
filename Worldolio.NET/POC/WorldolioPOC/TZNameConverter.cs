using TimeZoneConverter;

namespace WorldolioPOC
{
    internal class TZNameConverter
    {
        private static void OutputToConsole(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        public static void DisplayAllTzs()
        {
            var tzdata = new (int, string)[]
            {
                (-2147483584,"Azerbaijan Standard Time"),
                (-2147483583,"Middle East Standard Time"),
                (-2147483582,"Jordan Standard Time"),
                (-2147483581,"Central Standard Time (Mexico)"),
                (-2147483578,"Namibia Standard Time"),
                (-2147483577,"Georgian Standard Time"),
                (-2147483576,"Central Brazilian Standard Time"),
                (-2147483575,"Montevideo Standard Time"),
                (-2147483573,"Venezuela Standard Time"),
                (1,"Samoa Standard Time"),
                (2,"Hawaiian Standard Time"),
                (3,"Alaskan Standard Time"),
                (4,"Pacific Standard Time"),
                (10,"Mountain Standard Time"),
                (15,"US Mountain Standard Time"),
                (20,"Central Standard Time"),
                (25,"Canada Central Standard Time"),
                (33,"Central America Standard Time"),
                (35,"Eastern Standard Time"),
                (40,"US Eastern Standard Time"),
                (45,"SA Pacific Standard Time"),
                (50,"Atlantic Standard Time"),
                (55,"SA Western Standard Time"),
                (56,"Pacific SA Standard Time"),
                (60,"Newfoundland Standard Time"),
                (65,"E. South America Standard Time"),
                (70,"SA Eastern Standard Time"),
                (73,"Greenland Standard Time"),
//                (83,"Cabo Verde Standard Time"),
                (83,"Cape Verde Standard Time"),
                (85,"GMT Standard Time"),
                (90,"Greenwich Standard Time"),
                (95,"Central Europe Standard Time"),
                (100,"Central European Standard Time"),
                (105,"Romance Standard Time"),
                (110,"W. Europe Standard Time"),
                (113,"W. Central Africa Standard Time"),
                (120,"Egypt Standard Time"),
                (125,"FLE Standard Time"),
                (130,"GTB Standard Time"),
//                (135,"Jerusalem Standard Time"),
                (135,"Israel Standard Time"),
                (140,"South Africa Standard Time"),
                (145,"Russian Standard Time"),
                (150,"Arab Standard Time"),
                (155,"E. Africa Standard Time"),
                (158,"Arabic Standard Time"),
                (160,"Iran Standard Time"),
                (165,"Arabian Standard Time"),
                (170,"Caucasus Standard Time"),
                (175,"Afghanistan Standard Time"),
//                (180,"Russia TZ 4 Standard Time"),
                (180,"Ekaterinburg Standard Time"),
                (185,"West Asia Standard Time"),
                (190,"India Standard Time"),
                (193,"Nepal Standard Time"),
                (195,"Central Asia Standard Time"),
                (200,"Sri Lanka Standard Time"),
//                (201,"Novosibirsk Standard Time"),
                (201,"N. Central Asia Standard Time"),
                (203,"Myanmar Standard Time"),
                (205,"SE Asia Standard Time"),
//                (207,"Russia TZ 6 Standard Time"),
                (207,"North Asia Standard Time"),
                (210,"China Standard Time"),
//                (215,"Malay Peninsula Standard Time"),
                (215,"Singapore Standard Time"),
                (220,"Taipei Standard Time"),
                (225,"W. Australia Standard Time"),
//                (227,"Russia TZ 7 Standard Time"),
                (227,"North Asia East Standard Time"),
                (227,"Ulaanbaatar Standard Time"),
                (230,"Korea Standard Time"),
                (235,"Tokyo Standard Time"),
//                (240,"Russia TZ 8 Standard Time"),
                (240,"Yakutsk Standard Time"),
                (245,"AUS Central Standard Time"),
                (250,"Cen. Australia Standard Time"),
                (255,"AUS Eastern Standard Time"),
                (260,"E. Australia Standard Time"),
                (265,"Tasmania Standard Time"),
//                (270,"Russia TZ 9 Standard Time"),
                (270,"Vladivostok Standard Time"),
                (275,"West Pacific Standard Time"),
                (280,"Central Pacific Standard Time"),
                (285,"Fiji Standard Time"),
                (290,"New Zealand Standard Time"),
                (300,"Tonga Standard Time"),

//                (900,"Falkland Islands Standard Time"),
                (900,"SA Eastern Standard Time"),       // should be Atlantic/Stanley
                (902,"Mid-Atlantic Standard Time"),

//                (1000,"T�rkiye Standard Time"),
                (1000,"Turkey Standard Time"),
                (1001,"Bangladesh Standard Time"),
                (1002,"Cuba Standard Time"),
                (1003,"Haiti Standard Time"),
                (1004,"Libya Standard Time"),
                (1005,"Norfolk Standard Time"),
                (1006,"Sudan Standard Time"),
                (1007,"Syria Standard Time"),
                (1008,"Belarus Standard Time"),
                (1009,"Turks and Caicos Standard Time"),
                (1010,"Volgograd Standard Time"),
                (1011,"Yukon Standard Time"),

            };

            OutputToConsole($"win indx, win name, IANA name");
            foreach (var tz in tzdata)
            {
                var name = TZConvert.WindowsToIana(tz.Item2);
                OutputToConsole($"{tz.Item1},{tz.Item2},{name}");
            }
        }
    }
}
