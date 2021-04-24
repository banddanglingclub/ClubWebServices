﻿using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using AnglingClubWebServices.Interfaces;
using AnglingClubWebServices.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnglingClubWebServices.Data
{
    public class WaterRepository : RepositoryBase, IWaterRepository
    {
        private readonly ILogger<WaterRepository> _logger;

        public WaterRepository(
            IOptions<RepositoryOptions> opts,
            ILoggerFactory loggerFactory) : base(opts.Value.AWSAccessId, opts.Value.AWSSecret, opts.Value.SimpleDbDomain, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WaterRepository>();
        }

        public async Task AddOrUpdateWater()
        {
            var client = GetClient();

            BatchPutAttributesRequest request = new BatchPutAttributesRequest();
            request.DomainName = Domain;
            request.Items.Add(
                new ReplaceableItem
                {
                    Name = "Roecliffe Pond",
                    Attributes = new List<ReplaceableAttribute>
                    {
                        new ReplaceableAttribute { Name = "Type", Value = "Water", Replace = false },
                        new ReplaceableAttribute { Name = "Id", Value = "2", Replace = false },
                        new ReplaceableAttribute { Name = "AccessType", Value = "MembersAndGuests", Replace = false },
                        new ReplaceableAttribute { Name = "WaterType", Value = "Stillwater", Replace = false },
                    }
                }
            );

            try
            {
                BatchPutAttributesResponse response = await client.BatchPutAttributesAsync(request);
                _logger.LogDebug($"Water updated");
            }
            catch (AmazonSimpleDBException ex)
            {
                _logger.LogError(ex, $"Error Code: {ex.ErrorCode}, Error Type: {ex.ErrorType}");
                throw;
            }

        }


    }
}