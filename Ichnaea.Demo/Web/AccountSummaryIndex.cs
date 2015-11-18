using System;
using System.Collections.Generic;
using System.Linq;
using NEventStore;
using NEventStore.Persistence.RavenDB;
using Raven.Abstractions.Indexing;
using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Restall.Ichnaea.Demo.Accounts;
using Restall.Ichnaea.Demo.Web.Accounts;

namespace Restall.Ichnaea.Demo.Web
{
	public class AccountSummaryIndex: AbstractIndexCreationTask<AccountIdSurrogate, AccountSummary>
	{
		public AccountSummaryIndex()
		{
			this.Map = surrogates =>
				from surrogate in surrogates
				let commit = this.LoadDocument<RavenCommit>("Accounts/" + surrogate.SortCode + " / " + surrogate.AccountNumber + "/1")
				where commit.BucketId == "Accounts" && commit.StreamRevision == 1
				select new
					{
						Id = surrogate.SurrogateId,
						surrogate.SortCode,
						surrogate.AccountNumber,
						((AccountOpened) commit.Payload.First().Body).Holder,
						GetDetailsUri = new Uri("/accounts/" + surrogate.SurrogateId, UriKind.Relative)
				};

			this.StoreAllFields(FieldStorage.Yes);
		}
	}
}
