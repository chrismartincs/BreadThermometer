using System;
using System.Collections; 
using System.Collections.Generic;

// Structure for holding our data (Date&Time, Temperature, Humidity)
struct BreadData
{
    public string dateTime;
    public string temp;
    public string humidity;
}

public static void Run(Stream myBlob, string name, ILogger log)
{
    // Iterates through stream, skipping pass the header
    int low = 84;
    int high = 103;

    // List to hold our data
    List<BreadData> dataSet = new List<BreadData>();

    // Setup for while loop
    int counter = 0;
    bool flag = true;
    int length = 0;

    // Peak data for temps and humidity !Change based on enivornment!
    int peakTemp = 69;
    int peakHumidity = 49;

    // Proccess stream and put into string
    StreamReader reader = new StreamReader(myBlob); 
    string data = reader.ReadToEnd();

    length = data.Length;

    while (flag)
    {
        // In case of incorrect data to prevent infinite loop
        if (counter > 1000)
        {
            log.LogInformation("Too many entries in data");
            break;
        }

        // Take data and put into the list
        BreadData bd = new BreadData();
        bd.dateTime = data.Substring(low,19);
        bd.temp = data.Substring((low+20), (7));
        bd.humidity = data.Substring((low+28),(7));
        dataSet.Add(bd);

        // Move forward 35 bits
        low = low + 36;
        high = high + 36;

        // Break when at the end of data
        if (high >= length)
        {
            break;
        }
        counter = counter + 1;
    }

    int dataLen = dataSet.Count;

    // Search for first time peak temperature and peak humidity are hit
    for (int i = 0; i < dataLen; i++)
    {
        if (float.Parse(dataSet[i].temp) >= peakTemp)
        {
            if (float.Parse(dataSet[i].humidity) >= peakHumidity)
            {
                log.LogInformation("Bread is done at: " + dataSet[i].dateTime);
                break;
            }
        }
    }
}
