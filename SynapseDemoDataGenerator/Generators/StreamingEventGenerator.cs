using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using System.IO;
using ProtoBuf;
using System.Globalization;
using ExtensionMethods;

namespace SynapseDemoDataGenerator.Generators
{
    class StreamingEventGenerator : Generator<RetailTypes.StreamingEvent>
    {
        private int StartingUserId;
        private int EndingUserId;
        private string[] platformTypes = new string[4] { "browser", "mobile", "smarttv", "streamingdevice" };
        private Dictionary<string, string[]> platformNames = new Dictionary<string, string[]>
        {
            {"browser", new string[] { "Google Chrome", "Mozilla Firefox", "Microsoft Edge", "Microsoft Internet Explorer", "Safari" } },
            {"mobile", new string[] { "iOS", "Android" } },
            {"smarttv", new string[] { "Samsung", "LG", "Sony", "Vizio" } },
            {"streamingdevice", new string[] { "Roku", "FireTV", "AppleTV", "Chromecast", "Xbox", "Playstation" } }
        };

        public StreamingEventGenerator(int numberToGenerate, int userIdStart, int userIdEnd, int splitCount) : base(numberToGenerate, 0, splitCount)
        {
            //Passing the number to generate and starting ID to the base Generator class
            StartingUserId = userIdStart;
            EndingUserId = userIdEnd;
        }
        public override void Generate()
        {
            int DiskUseThreshold = Convert.ToInt32(Program.Configuration["UseDiskThreshold"]);
            Console.WriteLine("Generating Stream Events");

            //Putting this in to deal with memory limits around 10 million records
            if (GenerateCount < DiskUseThreshold)
            {
                Console.WriteLine("Beginning Stream Event generation in memory...");
                items = this.PrivateGenerator(GenerateCount);
                ItemsCreated = items.Count();
                Console.WriteLine("Stream Event generation complete.");
            }
            else
            {
                Console.WriteLine("Generating over {0} items, generating on disk, this may be slow...", DiskUseThreshold.ToString("N0", CultureInfo.InvariantCulture));
                Console.WriteLine("Beginning Stream Event generation on disk...");
                int filecount = 1;
                int numberLeft = GenerateCount;
                Directory.CreateDirectory("CacheData");

                // Handle large file scenarios with no split
                // Essentially if someone says they want 40mil records, but want them all in one file, we still need to decide where to cache at.
                int splitHold = SplitAmount;
                if (SplitAmount <= 0)
                    splitHold = DiskUseThreshold;

                while (numberLeft > 0)
                {
                    int createAmount = splitHold > numberLeft ? numberLeft : splitHold;
                    items = this.PrivateGenerator(createAmount);
                    using (FileStream fs = new FileStream("CacheData\\StreamingEventCache-" + filecount.ToString() + ".bin", FileMode.Create))
                    {
                        Serializer.Serialize(fs, items);
                    }
                    filecount++;
                    numberLeft = numberLeft - createAmount;
                    ItemsCreated += items.Count();
                }
                Console.WriteLine("Stream Event generation complete.");
            }

        }
        private List<RetailTypes.StreamingEvent> PrivateGenerator(int GenerateCount)
        {
            Random r = new Random();

            // Create a list of movie ids with related lengths to have consistent durations throughout a single generation
            int numberOfMovies = 1000;
            //Create our array of movies and give them a good shake
            var movieArray = Enumerable.Range(1, numberOfMovies).ToArray();
            r.Shuffle(movieArray);

            int[] movieLengthArray = new int[numberOfMovies];
            for (int i = 0; i < numberOfMovies; i++)
            {
                movieLengthArray[i] = r.Next(4680, 9000);
            }

            List<RetailTypes.StreamingEvent> events = new List<RetailTypes.StreamingEvent>();

            //Cut our requested generation count in half as we always have at least two messages per event
            int ActualGenerateCount = (int)(GenerateCount / 2);
            for (int i = 0; i < ActualGenerateCount; i++)
            {
                //Pick what sort of session
                int j = r.Next(3);
                //Make a constant session id
                Guid sessionId = Guid.NewGuid();
                //Pick our movie
                int movieId = movieArray[WeightedInteger(0, 999)];
                //How long is the movie?
                int movieLength = movieLengthArray[movieId-1];
                //Get that start time
                DateTime startMovie = RandomDateTime(r);
                //Get a random user
                int userId = WeightedInteger(StartingUserId, EndingUserId);
                //Pick our platform type
                string platformType = platformTypes[r.Next(platformTypes.Length)];
                //Based on the type, get the specific platform
                string platformName = platformNames[platformType][r.Next(platformNames[platformType].Length)];

                switch (j)
                {
                    //Started Movie, didn't Finish, No start/stop
                    case 0:
                        {
                            //Create our starting event
                            RetailTypes.StreamingEvent s = new RetailTypes.StreamingEvent
                            {
                                StreamingId = Guid.NewGuid(),
                                SessionId = sessionId,
                                EventTime = startMovie,
                                UserId = userId,
                                MediaId = movieId,
                                EventType = RetailTypes.EventType.Start,
                                Duration = 0,
                                PlatformType = platformType,
                                PlatformName = platformName
                            };
                            events.Add(s);

                            int watchTime = r.Next(movieLength - 1200);
                            DateTime watchTimeDT = startMovie.AddSeconds(watchTime);
                            //Create our ending event, any amount of time, minus 20 minutes
                            s = new RetailTypes.StreamingEvent
                            {
                                StreamingId = Guid.NewGuid(),
                                SessionId = sessionId,
                                EventTime = watchTimeDT,
                                UserId = userId,
                                MediaId = movieId,
                                EventType = RetailTypes.EventType.Stop,
                                Duration = watchTime,
                                PlatformType = platformType,
                                PlatformName = platformName
                            };
                            events.Add(s);
                        }
                        break;
                    //Started Movie, Finished, No start/stop
                    case 1:
                        {
                            //Create our starting event
                            RetailTypes.StreamingEvent s = new RetailTypes.StreamingEvent
                            {
                                StreamingId = Guid.NewGuid(),
                                SessionId = sessionId,
                                EventTime = startMovie,
                                UserId = userId,
                                MediaId = movieId,
                                EventType = RetailTypes.EventType.Start,
                                Duration = 0,
                                PlatformType = platformType,
                                PlatformName = platformName
                            };
                            events.Add(s);

                            //Create our completion event
                            s = new RetailTypes.StreamingEvent
                            {
                                StreamingId = Guid.NewGuid(),
                                SessionId = sessionId,
                                EventTime = startMovie.AddSeconds(movieLength),
                                UserId = userId,
                                MediaId = movieId,
                                EventType = RetailTypes.EventType.Complete,
                                Duration = movieLength,
                                PlatformType = platformType,
                                PlatformName = platformName
                            };
                            events.Add(s);
                        }
                        break;
                    //Started Movie, didn't Finish, With start/stop
                    case 2:
                        {
                            int streamTime = 0;
                            //Start with 10 to make sure they at least watched a few seconds
                            int watchTime = r.Next(10, movieLength - 1200);
                            double watchRatio = (double)r.Next(15, 51)/100;
                            int watchPer = (int)(watchTime * watchRatio);

                            DateTime startJump = startMovie;
                            //Fluff extra time on stream time
                            while (streamTime + 5 < watchTime)
                            {
                                //Create our starting event
                                RetailTypes.StreamingEvent s = new RetailTypes.StreamingEvent
                                {
                                    StreamingId = Guid.NewGuid(),
                                    SessionId = sessionId,
                                    EventTime = startJump,
                                    UserId = userId,
                                    MediaId = movieId,
                                    EventType = RetailTypes.EventType.Start,
                                    Duration = streamTime,
                                    PlatformType = platformType,
                                    PlatformName = platformName
                                };
                                events.Add(s);

                                int watchAmount = watchPer + streamTime;
                                startJump = startJump.AddSeconds(watchAmount);

                                //Create our ending event
                                s = new RetailTypes.StreamingEvent
                                {
                                    StreamingId = Guid.NewGuid(),
                                    SessionId = sessionId,
                                    EventTime = startJump,
                                    UserId = userId,
                                    MediaId = movieId,
                                    EventType = RetailTypes.EventType.Stop,
                                    Duration = watchAmount,
                                    PlatformType = platformType,
                                    PlatformName = platformName
                                };
                                events.Add(s);

                                streamTime = watchAmount;

                                //Wait between 2 minutes and 48 hours
                                startJump = startJump.AddSeconds(r.Next(120, 172800));
                            }
                        }
                        break;
                    //Started Movie, Finished, With start/stop
                    case 3:
                        {
                            int streamTime = 0;
                            //Start with 10 to make sure they at least watched a few seconds
                            int watchTime = r.Next(10, movieLength - 100);
                            double watchRatio = (double)r.Next(15, 51) / 100;
                            int watchPer = (int)(watchTime * watchRatio);

                            DateTime startJump = startMovie;

                            while (streamTime < watchTime - 300)
                            {
                                //Create our starting event
                                RetailTypes.StreamingEvent s = new RetailTypes.StreamingEvent
                                {
                                    StreamingId = Guid.NewGuid(),
                                    SessionId = sessionId,
                                    EventTime = startJump,
                                    UserId = userId,
                                    MediaId = movieId,
                                    EventType = RetailTypes.EventType.Start,
                                    Duration = streamTime,
                                    PlatformType = platformType,
                                    PlatformName = platformName
                                };
                                events.Add(s);

                                int watchAmount = watchPer + streamTime;
                                startJump = startJump.AddSeconds(watchAmount);
                                streamTime = watchAmount;
                                //If we are within 5 minutes of the end, assume we finished
                                if(streamTime >= watchTime - 300)
                                {
                                    //Create our completion event
                                    s = new RetailTypes.StreamingEvent
                                    {
                                        StreamingId = Guid.NewGuid(),
                                        SessionId = sessionId,
                                        EventTime = startJump,
                                        UserId = userId,
                                        MediaId = movieId,
                                        EventType = RetailTypes.EventType.Complete,
                                        Duration = movieLength,
                                        PlatformType = platformType,
                                        PlatformName = platformName
                                    };
                                    events.Add(s);
                                } else
                                {
                                    //Create our ending event
                                    s = new RetailTypes.StreamingEvent
                                    {
                                        StreamingId = Guid.NewGuid(),
                                        SessionId = sessionId,
                                        EventTime = startJump,
                                        UserId = userId,
                                        MediaId = movieId,
                                        EventType = RetailTypes.EventType.Stop,
                                        Duration = watchAmount,
                                        PlatformType = platformType,
                                        PlatformName = platformName
                                    };
                                    events.Add(s);
                                }

                                //Wait between 2 minutes and 48 hours
                                startJump = startJump.AddSeconds(r.Next(120, 172800));

                            }
                        }
                        break;
                } 
            }
            return events;
        }

        //Modified for seconds from here https://stackoverflow.com/a/194870/404006
        DateTime RandomDateTime(Random gen)
        {
            DateTime start = new DateTime(2015, 1, 1);
            int range = (int)(DateTime.Today - start).TotalSeconds;
            return start.AddSeconds(gen.Next(range));
        }
    }
}
