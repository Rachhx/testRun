﻿using System;
using System.Diagnostics;

namespace AstraTestWpf
{
    /// <summary>
    /// Helper class to calculate frame rate:
    /// FPS - frames per seconds.
    /// </summary>
    internal sealed class FrameRateCalculator
    {
        /// <summary>
        /// Creates frame rate calculator with specified parameters.
        /// You can reset existing object to initial state using <c>Reset()</c> method.
        /// </summary>
        /// <param name="refreshPeriod">How often FPS value should be refreshed. Default value is half of second.</param>
        /// <param name="periodCountForAveraging">Count of refresh periods that is used to calculate FPS. Default value is <c>5</c>.</param>
        /// <param name="smoothCoeff">Smoothing coefficient. Default value is <c>0f</c>, which means no smoothing. Value must be in range [0; 1).</param>
        public FrameRateCalculator(
            TimeSpan refreshPeriod = default(TimeSpan),
            int periodCountForAveraging = default(int),
            float smoothCoeff = default(float))
        {
            // default values
            if (refreshPeriod <= TimeSpan.Zero)
                refreshPeriod = TimeSpan.FromSeconds(0.5);
            if (periodCountForAveraging <= 0)
                periodCountForAveraging = 6;

            this.refreshPeriod = refreshPeriod;
            this.smoothCoeff = Math.Min(0.99f, Math.Max(0f, smoothCoeff));
            buffer = new Tuple<TimeSpan, long>[periodCountForAveraging];
        }

        /// <summary>
        /// How often FPS value should be refreshed as specified in call of object constructor.
        /// </summary>
        public TimeSpan RefreshPerion => refreshPeriod;

        /// <summary>
        /// Count of refresh periods that is used to calculate FPS.
        /// </summary>
        public int PeriodCountForAveraging => buffer.Length;

        /// <summary>
        /// Smoothing coefficient. <c>0f</c> means no smoothing. The closer to <c>1f</c>, the more aggressive smoothing.
        /// </summary>
        public float SmoothCoeff => smoothCoeff;

        /// <summary>
        /// Current value of frame rate: FPS - frames per second.
        /// </summary>
        public float FramesPerSecond => framesPerSecond;

        /// <summary>
        /// Registers new frame. Call this method each time you receive new frame from device.
        /// </summary>
        /// <returns><c>true</c> - value of <c>FramesPerSecond</c> is updated, <c>false</c> - else.</returns>
        public bool RegisterFrame()
        {
            lock (buffer)
            {
                if (!stopwatch.IsRunning)
                {
                    stopwatch.Start();
                    return false;
                }

                frameCounter++;

                var moment = stopwatch.Elapsed;
                if (moment < nextTimeSpan)
                    return false;

                nextTimeSpan = moment.Add(refreshPeriod);

                var old = buffer[bufferIndex];
                buffer[bufferIndex] = Tuple.Create(moment, frameCounter);
                bufferIndex = (bufferIndex + 1) % buffer.Length;

                if (old == null)
                    return false;

                var timeDiffSec = (moment - old.Item1).TotalSeconds;
                if (timeDiffSec <= double.Epsilon)
                    return false;

                var frameDiff = frameCounter - old.Item2;
                var newFps = (float)(frameDiff / timeDiffSec);
                framesPerSecond = framesPerSecond > 0f
                    ? framesPerSecond * smoothCoeff + (1f - smoothCoeff) * newFps
                    : newFps;
            }

            return true;
        }

        /// <summary>
        /// Resets object to initial state.
        /// </summary>
        public void Reset()
        {
            lock (buffer)
            {
                Array.Clear(buffer, 0, buffer.Length);
                stopwatch.Reset();
                bufferIndex = 0;
                nextTimeSpan = refreshPeriod;
                frameCounter = 0;
                framesPerSecond = 0;
            }
        }

        private readonly TimeSpan refreshPeriod;
        private readonly float smoothCoeff;
        private readonly Tuple<TimeSpan, long>[] buffer;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private int bufferIndex = 0;
        private TimeSpan nextTimeSpan;
        private long frameCounter;
        private volatile float framesPerSecond;
    }
}
