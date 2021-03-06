﻿namespace CallForm.Core.Services
{
    /// <summary>Creates the interface for <see cref="LocationService"/>.
    /// </summary>
    /// <remarks>Signatures of methods, properties, events and/or indexers</remarks>
    public interface ILocationService
    {
        /// <summary>Attempts to get the current coordinates.
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lng">Longitude</param>
        /// <returns></returns>
        bool TryGetLatestLocation(out double lat, out double lng);
    }
}