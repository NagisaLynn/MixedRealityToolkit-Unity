﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityBoundary = UnityEngine.Experimental.XR.Boundary;

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene.
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Boundary/BoundarySystemGettingStarted.html")]
    public class MixedRealityBoundarySystem : BaseBoundarySystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The configuration profile for the service.</param>
        public MixedRealityBoundarySystem(MixedRealityBoundaryVisualizationProfile profile) : base(profile) { }

        #region IMixedRealityService Implementation

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Boundary System";

        /// <inheritdoc/>
        public override void Initialize()
        {
            if (!Application.isPlaying || !XRDevice.isPresent) { return; }

            base.Initialize();

            UnityBoundary.visible = true;
        }

        #endregion IMixedRealityService Implementation

        /// <inheritdoc/>
        protected override List<Vector3> GetBoundaryGeometry()
        {
            // Boundaries are supported for Room Scale experiences only.
            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                return null;
            }

            // Get the boundary geometry.
            var boundaryGeometry = new List<Vector3>(0);

            if (!UnityBoundary.TryGetGeometry(boundaryGeometry, UnityBoundary.Type.TrackedArea) || boundaryGeometry.Count == 0)
            {
                return null;
            }

            return boundaryGeometry;
        }

        /// <inheritdoc/>
        protected override void SetTrackingSpace()
        {
            if (Application.isPlaying)
            {
                TrackingSpaceType trackingSpace;

                // In current versions of Unity, there are two types of tracking spaces. For boundaries, if the scale
                // is not Room or Standing, it currently maps to TrackingSpaceType.Stationary.
                switch (Scale)
                {
                    case ExperienceScale.Standing:
                    case ExperienceScale.Room:
                        trackingSpace = TrackingSpaceType.RoomScale;
                        break;

                    case ExperienceScale.OrientationOnly:
                    case ExperienceScale.Seated:
                    case ExperienceScale.World:
                        trackingSpace = TrackingSpaceType.Stationary;
                        break;

                    default:
                        trackingSpace = TrackingSpaceType.Stationary;
                        Debug.LogWarning("Unknown / unsupported ExperienceScale. Defaulting to Stationary tracking space.");
                        break;
                }

                InputTracking.disablePositionalTracking = Scale == ExperienceScale.OrientationOnly;

                if (!XRDevice.SetTrackingSpaceType(trackingSpace))
                {
                    Debug.LogWarning($"MRTK was unable to set Tracking Space to {trackingSpace}");
                }
            }
        }

        #region Obsolete

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The configuration profile for the service.</param>
        /// <param name="scale">The application's configured <see cref="Utilities.ExperienceScale"/>.</param>
        [System.Obsolete("This constructor is obsolete. The IMixedRealityBoundarySystem no longer captures ExperienceScale. Use MixedRealityToolkit.AppScale instead.")]
        public MixedRealityBoundarySystem(
            MixedRealityBoundaryVisualizationProfile profile,
            ExperienceScale scale) : base(profile, scale) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        /// <param name="scale">The application's configured <see cref="Utilities.ExperienceScale"/>.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public MixedRealityBoundarySystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealityBoundaryVisualizationProfile profile,
            ExperienceScale scale) : this(profile, scale)
        {
            Registrar = registrar;
        }

        #endregion
    }
}
