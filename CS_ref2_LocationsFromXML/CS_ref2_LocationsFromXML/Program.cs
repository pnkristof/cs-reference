using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CS_ref2_LocationsFromXML
{
    class Program
    {
        //this file contains all the classes and functions

        static void Main(string[] args)
        {
            XDocument xd = XDocument.Load("mondial-3.0.xml");

            var Countries = GetData.GetCountries(xd);

            //1. task
            //all densities ordered descending
            var PopDensity = from x in Countries
                             orderby x.TotalPopulation / x.Area descending
                             select new
                             {
                                 Country = x,
                                 Density = x.TotalPopulation / x.Area
                                 //density = population/area (person/km^2)
                             };

            //foreach (var item in PopDensity)
            //{
            //    Console.WriteLine(item);
            //}

            Console.WriteLine("1. Task\n" +
                               "Country with the highest density: {0} ({1} person/km^2)",
                               PopDensity.First().Country.Name,
                               PopDensity.First().Density);


            Console.WriteLine();
            //2. task
            //countries with democracy
            Regex demRgx = new Regex("democra(tic|cy)");
            var DemocraticCountries = from x in Countries
                                      where demRgx.IsMatch(x.Government) //does the government field contain democratic/democracy expression
                                      select x;


            Console.WriteLine("2. task\n" +
                              "{0}% of the contries are democratic."
                              , Math.Round(100 * (double)DemocraticCountries.Count() / Countries.Count(), 2));
            //count of democratic countries/count of every country (2 decimals)

            Console.WriteLine("Count of contries: " + Countries.Count() + " Democracy: " + DemocraticCountries.Count());


            Console.WriteLine();
            //3. task
            var CountOfGroups = (from x in Countries
                                 orderby x.Religions.Count() + x.Ethnics.Count() descending
                                 select new
                                 {
                                     Name = x.Name,
                                     Groups = x.Religions.Count() + x.Ethnics.Count()
                                 }).Take(5); //the first 5 just for example

            Console.WriteLine("3. task\n" +
                              "countries ordered by the count of the ethnical and religional groups:");

            foreach (var item in CountOfGroups)
            {
                Console.WriteLine("{0}: {1}", item.Name, item.Groups);
            }




            //foreach (var item in countries.First().Religions)
            //{
            //    Console.WriteLine(item.Name + " " + item.Percentage);
            //}



            Console.WriteLine();
            //4. task
            var BordersLength = (from x in Countries
                                 select new
                                 {
                                     Name = x.Name,
                                     BorderLength = x.Borders.Sum(y => y.Length)
                                 }).Take(5);//the first 5 just for example

            Console.WriteLine("4. task\n" +
                              "length of the borders of the countries:");

            foreach (var item in BordersLength)
            {
                Console.WriteLine("{0}: {1}", item.Name, item.BorderLength);
            }


            Console.WriteLine();
            //5. task
            //rivers ordered by occurence
            var Rivers = GetData.GetRivers(xd);
            var RiversOrderedByLocation = (from x in Rivers
                                           orderby x.Locations.Count() descending
                                           select x)
                                          .Take(5); //the first 5





            Console.WriteLine("5. task\n" +
                              "5 rivers that run across the most countries:");

            foreach (var river in RiversOrderedByLocation)
            {
                Console.WriteLine("{0}:\t{1}", river.Name, river.Locations.Count());
            }


            Console.WriteLine();
            //6. task
            //var Waters = GetData.GetWaters(xd); every water

            
            //locations of the waters in the countries ordered descending
            //- every water contains the ID of the country only once
            //for example the Donau, which contains 19 'located' element, actualy contains only 2 different ones
            var WaterLocsOrderedByCount = (from x in GetData.GetWaters(xd).SelectMany(y => y.Locations)
                                           group x by x into g
                                           orderby g.Count() descending
                                           select new
                                           {
                                               Name = Countries.Where(y => y.ID == g.Key).First().Name,
                                               ID = g.Key,
                                               Count = g.Count()
                                           }).Take(5); //az első 5



            Console.WriteLine("6. task\n" +
                              "occurence of the waters in the contries:");

            foreach (var location in WaterLocsOrderedByCount)
            {
                Console.WriteLine("{0}:\t{1}", location.Name, location.Count);
            }



            Console.WriteLine();
            //7. task
            //rivers, that end in a see -> choose the rivers, which ToID can be found in the IDs of the sees

            //var RiversToSea = from x in rivers
            //                  where
            //                  (
            //                      from y in GetData.GetSeas(xd)
            //                      select y.ID
            //                  ).Contains(x.ToID)
            //                  select x;


            var RiversToSea = (from x in Rivers
                               where
                               (
                                   from y in GetData.GetSeas(xd)
                                   select y.ID
                               ).Contains(x.ToID)
                               group x by x.ToID into g
                               orderby g.Count() descending
                               select new
                               {
                                   Name = GetData.GetSeas(xd).Where(y => y.ID == g.Key).First().Name,
                                   Count = g.Count()
                               }).Take(5); //az első 5

            Console.WriteLine("7. task\n" +
                              "sees, with the most rivers end in:");
            foreach (var river in RiversToSea)
            {
                Console.WriteLine("{0}:\t{1}", river.Name, river.Count);
            }


            Console.WriteLine();
            //8. task
            //all of the ethnics
            //var Ethnics = countries.SelectMany(x => x.Ethnics);
            //summarized, ordered descending
            var SumEthnics = (from x in Countries.SelectMany(x => x.Ethnics)
                              group x by x.Name into g
                              orderby g.Sum(y => y.Quantity) descending
                              select new
                              {
                                  Name = g.Key,
                                  Sum = g.Sum(x => x.Quantity)
                              }).Take(5);

            Console.WriteLine("8. task\n" +
                              "ethnics of the world:");
            foreach (var item in SumEthnics)
            {
                Console.WriteLine("{0}:\t{1:N}", item.Name, item.Sum);
            }

            Console.ReadLine();
        }



    }

    class Country
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string CapitalID { get; set; }
        public int TotalPopulation { get; set; }
        public int Area { get; set; }
        public string Government { get; set; }
        public IEnumerable<Group> Religions { get; set; }
        public IEnumerable<Group> Ethnics { get; set; }
        public IEnumerable<Border> Borders { get; set; }
    }


    class Water
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Locations { get; set; }
    }

    class River : Water
    {
        public string ToID { get; set; }
        //public int Length { get; set; }
    }



    enum GroupType { Ethnical, Religious }
    class Group
    {
        GroupType Type { get; set; }
        public string Name { get; set; }
        public double Percentage { get; set; }
        public int Quantity { get; set; }

        public Group(GroupType type, string name, double percentage, int quantity)
        {
            Type = type;
            Name = name;
            Percentage = percentage;
            Quantity = quantity;
        }
    }

    class Border
    {
        public string ID1 { get; set; }
        public string ID2 { get; set; }
        public double Length { get; set; }

        public Border(string id1, string id2, double length)
        {
            ID1 = id1;
            ID2 = id2;
            Length = length;
        }
    }


    class GetData
    {


        public static IEnumerable<Country> GetCountries(XDocument xd)
        {
            var countries = from x in xd.Descendants("country")
                            select new Country
                            {
                                Name = x.Attribute("name").Value,
                                ID = x.Attribute("id").Value,
                                CapitalID = x.Attribute("capital").Value,
                                TotalPopulation = int.Parse(x.Attribute("population").Value),
                                Area = int.Parse(x.Attribute("total_area").Value),
                                Government = x.Attribute("government").Value,

                                Religions = from y in x.Elements("religions")
                                            select new Group(GroupType.Religious, y.Value, double.Parse(y.Attribute("percentage").Value.Replace('.', ',')),//type, name, percent
                                            (int)(int.Parse(x.Attribute("population").Value) * double.Parse(y.Attribute("percentage").Value.Replace('.', ',')) / 100)),//quantity

                                Ethnics = from y in x.Elements("ethnicgroups")
                                          select new Group(GroupType.Ethnical, y.Value, double.Parse(y.Attribute("percentage").Value.Replace('.', ',')),//type, name, percent
                                          (int)(int.Parse(x.Attribute("population").Value) * double.Parse(y.Attribute("percentage").Value.Replace('.', ',')) / 100)),//quantity

                                Borders = from y in x.Elements("border")
                                          select new Border(x.Attribute("id").Value, y.Attribute("country").Value, double.Parse(y.Attribute("length").Value.Replace('.', ',')))//contry1, contry2, length
                                          //the ID field is unnecessary, a collection with double variables would be enough

                            };

            return countries;

        }

        public static IEnumerable<River> GetRivers(XDocument xd)
        {

            var rivers = from x in xd.Descendants("river")
                         select new River
                         {
                             Name = x.Attribute("name").Value,
                             ID = x.Attribute("id").Value,
                             //Length = int.Parse(x.Attribute("length").Value),    unnecessary
                             Locations = (from y in x.Elements("located")
                                          select y.Attribute("country").Value).Distinct(),
                             ToID = x.Element("to").Attribute("water").Value
                         };
            return rivers;


        }

        public static IEnumerable<Water> GetSeas(XDocument xd)
        {
            var seas = from x in xd.Descendants("sea")
                       select new Water
                       {
                           Name = x.Attribute("name").Value,
                           ID = x.Attribute("id").Value,
                           Locations = (from y in x.Elements("located")
                                        select y.Attribute("country").Value).Distinct()
                       };

            return seas;
        }

        public static IEnumerable<Water> GetLakes(XDocument xd)
        {
            var lakes = from x in xd.Descendants("lake")
                        select new Water
                        {
                            Name = x.Attribute("name").Value,
                            ID = x.Attribute("id").Value,
                            Locations = (from y in x.Elements("located")
                                         select y.Attribute("country").Value).Distinct()
                        };

            return lakes;
        }

        public static IEnumerable<Water> GetWaters(XDocument xd)
        {
            var rivers = GetRivers(xd);
            var lakes = GetLakes(xd);
            var seas = GetSeas(xd);

            return rivers.Concat(lakes).Concat(seas);
        }
    }



}
