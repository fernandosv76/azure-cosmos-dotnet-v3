﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Pagination
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using Microsoft.Azure.Cosmos.Query.Core.Monads;
    using Microsoft.Azure.Cosmos.Tracing;
    using Microsoft.Azure.Documents;

    /// <summary>
    /// Has the ability to page through a partition range.
    /// </summary>
    internal abstract class PartitionRangePageAsyncEnumerator<TPage, TState> : IAsyncEnumerator<TryCatch<TPage>>
        where TPage : Page<TState>
        where TState : State
    {
        private CancellationToken cancellationToken;

        protected PartitionRangePageAsyncEnumerator(FeedRangeInternal range, CancellationToken cancellationToken, TState state = default)
        {
            this.Range = range ?? throw new ArgumentNullException(nameof(range));
            this.State = state;
            this.cancellationToken = cancellationToken;
        }

        public FeedRangeInternal Range { get; }

        public TryCatch<TPage> Current { get; private set; }

        public TState State { get; private set; }

        public bool HasStarted { get; private set; }

        private bool HasMoreResults => !this.HasStarted || (this.State != default);

        public ValueTask<bool> MoveNextAsync()
        {
            return this.MoveNextAsync(NoOpTrace.Singleton);
        }

        public async ValueTask<bool> MoveNextAsync(ITrace trace)
        {
            if (trace == null)
            {
                throw new ArgumentNullException(nameof(trace));
            }

            using (ITrace childTrace = trace.StartChild(name: $"{this.Range} move next", TraceComponent.Pagination, TraceLevel.Info))
            {
                if (!this.HasMoreResults)
                {
                    return false;
                }

                this.Current = await this.GetNextPageAsync(trace: childTrace, cancellationToken: this.cancellationToken);
                if (this.Current.Succeeded)
                {
                    this.State = this.Current.Result.State;
                    this.HasStarted = true;
                }

                return true;
            }
        }

        protected abstract Task<TryCatch<TPage>> GetNextPageAsync(ITrace trace, CancellationToken cancellationToken);

        public abstract ValueTask DisposeAsync();

        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }
    }
}
