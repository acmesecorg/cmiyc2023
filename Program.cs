var lines = File.ReadLines(@"cmiyc-2023_01_pro.yaml");
var count = 0;

var userNameKey = "";
var givenName = "";
var surName = "";
var created = "";
var city = "";
var phone = "";
var company = "";
var department = "";
var passwordHash = "";
var plain = "";

var outputHashes = new List<string>();
var outputWords = new List<string>();
var outputGivenNames = new List<string>();
var outputSurNames = new List<string>();

var outputCsv = new List<string>();

var founds = new Dictionary<string, string>();

//Load all founds
var currentDirectory = Directory.GetCurrentDirectory();
var files = Directory.GetFiles(currentDirectory, "*.found");

foreach (var  file in files)
{
    var fileLines = File.ReadAllLines(file);
    foreach (var fileLine in fileLines)
    {
        var fileLineSplits = fileLine.Split(':');
        founds.Add(fileLineSplits[0], fileLineSplits[1]);
    }
}



outputCsv.Add("User,GivenName,Surname,Created,City,Phone,Company,Department,PasswordHash,Plain");

foreach (var line in lines)
{
    if (line == "---") continue;
    if (line == "users:") continue;

    var splits = line.Split('"');

    if (line.StartsWith("  - "))
    {
        count = 0;
        userNameKey = splits[1];

        givenName = "";
        surName = "";
        created = "";
        city = "";
        phone = "";
        company = "";
        department = "";
        passwordHash = "";
        plain = "";
    }

    var prop = splits[0].Trim().Split(':')[0];

    if (prop == "GivenName") givenName = splits[1];
    if (prop == "SurName") surName = splits[1];
    if (prop == "Created") created = splits[1];
    if (prop == "City") city = splits[1];
    if (prop == "Phone") phone = splits[1];
    if (prop == "Company") company = splits[1];
    if (prop == "Department") department = splits[1];
    if (prop == "PasswordHash")
    {
        passwordHash = splits[1];

        //See if we have a plain in the founds
        founds.TryGetValue(passwordHash, out plain);

        outputGivenNames.Add(givenName);
        outputSurNames.Add(surName);

        //Write out csv
        outputCsv.Add($"{userNameKey},{givenName},{surName},{created},{city},{phone},{company},{department},{passwordHash},{plain}");

        //Check if we have bcrypt then write out -a 9 files
        if (passwordHash.StartsWith("$2a$08$"))
        {
            outputHashes.Add(passwordHash);
            outputWords.Add(userNameKey);

            outputHashes.Add(passwordHash);
            outputWords.Add(givenName);

            outputHashes.Add(passwordHash);
            outputWords.Add(surName);

            outputHashes.Add(passwordHash);
            outputWords.Add(city);

            outputHashes.Add(passwordHash);
            outputWords.Add(phone);

            outputHashes.Add(passwordHash);
            outputWords.Add(company);

            outputHashes.Add(passwordHash);
            outputWords.Add(department);
        }
    }

    count++;


}

//Dump out the hashes and words
File.WriteAllLines(@"3200.hashes", outputHashes.ToArray());
File.WriteAllLines(@"3200.words", outputWords.ToArray());

//Dump out specific word lists
File.WriteAllLines(@"givennames.dic", outputGivenNames.ToArray());
File.WriteAllLines(@"surnames.dic", outputSurNames.ToArray());

//Dump out csv with founds
File.WriteAllLines(@"2023.csv", outputCsv.ToArray());


