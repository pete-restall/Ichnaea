using System;
using System.Linq;
using NEventStore.Persistence.RavenDB;
using Raven.Client.Indexes;
using Restall.Ichnaea.Demo.Accounts;
using Restall.Ichnaea.Demo.Web.Accounts;

namespace Restall.Ichnaea.Demo.Web
{
	public class AccountSummaryIndex: AbstractIndexCreationTask<RavenCommit, AccountSummary>
	{
		public AccountSummaryIndex()
		{
			this.Map = commits =>
				from commit in commits
				where commit.BucketId == "Accounts" && commit.StreamRevision == 1
				let surrogate = this.LoadDocument<AccountIdSurrogate>(commit.StreamId)
				select new
					{
						Id = surrogate.SurrogateId,
						surrogate.SortCode,
						surrogate.AccountNumber,
						((AccountOpened) commit.Payload.First().Body).Holder,
						GetDetailsUri = new Uri("/accounts/" + surrogate.SurrogateId, UriKind.Relative)
					};
		}
	}
}
