// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

namespace TcPlayer.Engine.Models
{
    internal class AsyncProcesssResult
    {
        public bool Completed { get; init; }
        public int? ExitCode { get; init; }
        public string Output { get; init; }

        public AsyncProcesssResult()
        {
            Output = string.Empty;
        }
    }
}
