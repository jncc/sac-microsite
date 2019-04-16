using System;
using System.Collections.Generic;
using System.Data.Odbc;
using JNCC.Microsite.SAC.Models.Data;

namespace JNCC.Microsite.SAC.Data
{
    public class DatabaseOperations
    {
        private readonly string _databasePath;
        private readonly static string _habitatFilter = "H*";
        private readonly static string _speciesFilter = "S*";

        public DatabaseOperations(string databasePath)
        {
            _databasePath = databasePath;
        }

        private DatabaseConnection GetDatabaseconnection()
        {
            return new DatabaseConnection(_databasePath);
        }
        public List<Site> GetFullSACList()
        {
            List<Site> sacs = new List<Site> { };

            using (DatabaseConnection conn = GetDatabaseconnection())
            {
                OdbcCommand cmd = conn.CreateCommand("{CALL Select_all_SACs_any_status}");
                using (OdbcDataReader reader = conn.RunCommand(cmd))
                {
                    // Basic SAC List
                    while (reader.Read())
                    {
                        // var EUCode = reader.GetString(1);
                        // var Country = reader.GetString(2);
                        // var Name = reader.GetString(3);
                        // var CountryFull = reader.GetString(4);
                        // var Area = reader.GetDouble(5);
                        // var GridReference = reader.IsDBNull(6) ? null : reader.GetString(6);
                        // var LocalAuthority = reader.GetString(7);
                        // var StatusCode = reader.GetInt32(8);
                        // var StatusShort = reader.GetString(9);
                        sacs.Add(new Site
                        {
                            EUCode = reader.GetString(1),
                            Country = reader.GetString(2),
                            Name = reader.GetString(3),
                            CountryFull = reader.GetString(4),
                            Area = reader.GetDouble(5),
                            GridReference = reader.IsDBNull(6) ? null : reader.GetString(6),
                            LocalAuthority = reader.GetString(7),
                            StatusCode = reader.GetInt32(8),
                            StatusShort = reader.GetString(9)
                        });
                    }

                }
            }
            // More detailed SAC List (no information feature info yet)
            foreach (Site site in sacs)
            {
                using (DatabaseConnection conn = GetDatabaseconnection())
                {
                    OdbcCommand cmd = conn.CreateCommand("{CALL Select_Site_Data_by_EUcode(?)}");
                    var prm = cmd.Parameters.Add("SAC_EU_CODE", OdbcType.Char, 9);
                    prm.Value = site.EUCode;

                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();

                        site.Latitude = reader.GetDouble(6);
                        site.Longitude = reader.GetDouble(7);
                        if (!reader.IsDBNull(8))
                        {
                            site.XCoord = reader.GetInt32(8);
                        }
                        if (!reader.IsDBNull(9))
                        {
                            site.YCoord = reader.GetInt32(9);
                        }
                        site.LinkText = reader.IsDBNull(11) ? null : reader.GetString(11);
                        site.StatusLong = reader.GetString(14);
                    }

                    List<SiteFeature> features = new List<SiteFeature> { };
                    cmd = conn.CreateCommand("{CALL SAC_Features_occurrences(?)}");
                    prm = cmd.Parameters.Add("SAC_EU_CODE", OdbcType.Char, 9);
                    prm.Value = site.EUCode;
                    // Several results can appear here

                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            features.Add(new SiteFeature
                            {
                                Code = reader.GetString(0),
                                Name = reader.GetString(1),
                                InterestStatus = reader.GetString(4),
                                InterestStatusLong = reader.GetString(5),
                                GlobalGrade = reader.GetString(6),
                                PrimaryText = reader.IsDBNull(7) ? null : reader.GetString(7),
                                SecondaryText = reader.IsDBNull(8) ? null : reader.GetString(8),
                                LocalAuthority = reader.GetString(9),
                                LayTitle = reader.GetString(10),
                                Priority = reader.GetBoolean(11)
                            });
                        }
                    }

                    site.Features = features;
                }


                using (DatabaseConnection conn = GetDatabaseconnection())
                {
                    OdbcCommand cmd = conn.CreateCommand("{CALL Select_SAC_Site_Character(?)}");
                    OdbcParameter prm = cmd.Parameters.Add("SAC_EU_CODE", OdbcType.Char, 9);
                    prm.Value = site.EUCode;

                    List<SiteCharacter> character = new List<SiteCharacter> { };

                    // Several results can appear here
                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            character.Add(new SiteCharacter
                            {
                                Character = reader.GetString(1),
                                Coverage = reader.IsDBNull(2) ? Double.NaN : reader.GetDouble(2)
                            });
                        }
                    }

                    site.Character = character;
                }
            }

            return sacs;
        }

        public List<InterestFeature> GetHabitatInformationFeatureList()
        {
            List<InterestFeature> habitats = GetInterestFeature(_habitatFilter);
            return habitats;
        }

        public List<InterestFeature> GetSpeciesInformationFeatureList()
        {
            List<InterestFeature> species = GetInterestFeature(_speciesFilter);
            return species;
        }


        private List<InterestFeature> GetInterestFeature(string Filter)
        {
            List<InterestFeature> features = new List<InterestFeature> { };
            using (DatabaseConnection conn = GetDatabaseconnection())
            {
                // Oddity from the way the query works, this covers species and habitat interest
                // features, not just species as the query name implies
                OdbcCommand cmd = conn.CreateCommand("{CALL Select_all_species_features(?)}");
                OdbcParameter prm = cmd.Parameters.Add("prefix", OdbcType.Char, 5);
                prm.Value = Filter;

                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    // Basic Feature List
                    while (reader.Read())
                    {
                        var Code = reader.GetString(0);
                        var Name = reader.GetString(1);
                        var LayTitle = reader.GetString(2);
                        var SectionNumber = reader.GetDouble(3);
                        var SectionTitle = reader.GetString(4);
                        var InterestGroup = reader.IsDBNull(5) ? null : reader.GetString(5);
                        var Priority = reader.GetBoolean(6);
                        var Total = reader.GetInt32(8);

                        features.Add(new InterestFeature
                        {
                            Code = reader.GetString(0),
                            Name = reader.GetString(1),
                            LayTitle = reader.GetString(2),
                            SectionNumber = reader.GetDouble(3),
                            SectionTitle = reader.GetString(4),
                            InterestGroup = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Priority = reader.GetBoolean(6),
                            Total = reader.GetInt32(8)

                        });
                    }
                }
            }


            foreach (var feature in features)
            {
                using (DatabaseConnection conn = GetDatabaseconnection())
                {
                    // Oddity from the way the query works, this covers species and habitat interest
                    // features, not just species as the query name implies
                    OdbcCommand cmd = conn.CreateCommand("{CALL Occurrences_by_Feature_INT_CODE(?)}");
                    OdbcParameter prm = cmd.Parameters.Add("prefix", OdbcType.Char, 5);
                    prm.Value = feature.Code;

                    List<InterestFeatureOccurrence> occurences = new List<InterestFeatureOccurrence> { };

                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var SiteName = reader.GetString(2);
                            var SiteCode = reader.GetString(3);
                            var InterestStatus = reader.GetString(4);
                            var InterestStatusLong = reader.GetString(5);
                            var GlobalGrade = reader.GetString(6);
                            var PrimaryText = reader.IsDBNull(7) ? null : reader.GetString(7);
                            var SecondaryText = reader.IsDBNull(8) ? null : reader.GetString(8);
                            var LocalAuthority = reader.GetString(9);

                            occurences.Add(new InterestFeatureOccurrence
                            {
                                SiteName = reader.GetString(2),
                                SiteCode = reader.GetString(3),
                                InterestStatus = reader.GetString(4),
                                InterestStatusLong = reader.GetString(5),
                                GlobalGrade = reader.GetString(6),
                                PrimaryText = reader.IsDBNull(7) ? null : reader.GetString(7),
                                SecondaryText = reader.IsDBNull(8) ? null : reader.GetString(8),
                                LocalAuthority = reader.GetString(9)
                            });
                        }
                    }

                    feature.Occurrences = occurences;
                }
            }

            return features;
        }
    }
}