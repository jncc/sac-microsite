using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text.RegularExpressions;
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
                    OdbcCommand cmd = conn.CreateCommand(String.Format("SELECT FEATURE_DESCRIPTION, EU_STATUS, UK_STATUS, RATIONALE FROM ASP_INTEREST_FEATURES WHERE INT_CODE LIKE '{0}'", feature.Code));

                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        // Single result expected
                        reader.Read();

                        feature.FeatureDescription = Regex.Replace(reader.GetString(0), @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);
                        feature.EUStatus = Regex.Replace(reader.GetString(1), @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);
                        feature.UKStatus = reader.GetString(2);
                        feature.Rationale = reader.IsDBNull(3) ? null : reader.GetString(3);
                    }

                }

                using (DatabaseConnection conn = GetDatabaseconnection())
                {
                    OdbcCommand cmd = conn.CreateCommand("{CALL ASP312_feature_detail(?)}");
                    OdbcParameter prm = cmd.Parameters.Add("FeatureIntCode", OdbcType.Char, 5);
                    prm.Value = feature.Code;

                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();

                        feature.MapData = new InterestFeatureMapData()
                        {
                            MapSources = reader.IsDBNull(5) ? null : reader.GetString(5),
                            MapExplanation = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Units = reader.IsDBNull(7) ? null : reader.GetString(7),
                            England = reader.IsDBNull(8) ? null : reader.GetString(8),
                            Scotland = reader.IsDBNull(9) ? null : reader.GetString(9),
                            Wales = reader.IsDBNull(10) ? null : reader.GetString(10),
                            NorthernIreland = reader.IsDBNull(11) ? null : reader.GetString(11),
                            UKOffshoreWaters = reader.IsDBNull(12) ? null : reader.GetString(12),
                            TotalUkPopulation = reader.IsDBNull(13) ? null : reader.GetString(13)
                        };
                    }

                }

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