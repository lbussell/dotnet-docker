// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DotNet.Docker.SizeReportGenerator.Model;

internal record Image(
    string Name,
    string BaseImage,
    DeploymentType DeploymentType,
    int UncompressedSize,
    int CompressedSize)
{
    public string GetSizeSavings(int baselineSize)
    {
        throw new NotImplementedException();
    }
}
