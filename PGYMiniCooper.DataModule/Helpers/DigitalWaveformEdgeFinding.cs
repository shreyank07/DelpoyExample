using PGYMiniCooper.DataModule.Structure;
using Prodigy.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Helpers
{
    /// <summary>
    /// Edge finding logic for digital waveform
    /// </summary>
    public class DigitalWaveformEdgeFinding
    {
        private readonly IList<DiscreteWaveForm> waveform;

        public DigitalWaveformEdgeFinding(IList<DiscreteWaveForm> waveform)
        {
            this.waveform = waveform;
        }

        /// <summary>
        /// Gets the state of waveform at the specified index.
        /// </summary>
        /// <param name="channel">Channel for which to state</param>
        /// <param name="waveformIndex">Index at which to find state</param>
        /// <returns>State of the waveform - High / Low, Quasi is not supported for digital waveform</returns>
        public eWfmState GetWaveformState(eChannles channel, int waveformIndex)
        {
            if (waveformIndex < 0 || waveformIndex > waveform.Count)
                throw new ArgumentOutOfRangeException(nameof(waveformIndex));

            return waveform[waveformIndex].GetChannelState(channel) == 0 ? eWfmState.LOW : eWfmState.HIGH;
        }

        /// <summary>
        /// Gets the next data edge.
        /// </summary>
        /// <param name="channel">Channel for which to find edge</param>
        /// <param name="waveformIndex">Index from which to start searching</param>
        /// <param name="edgeType">returns the edge type of the found edge. Default is NO_EDGE</param>
        /// <param name="waveformIndexAtEdge">returns the waveform index when the edge is found</param>
        /// <returns>return true when edge is found</returns>
        /// <exception cref="ArgumentOutOfRangeException">Waveform index must be in the range of the waveform</exception>
        public bool GetNextEdge(eChannles channel, int waveformIndex, out eEdgeType edgeType, out int waveformIndexAtEdge)
        {
            if (waveformIndex < 0 || waveformIndex > waveform.Count)
                throw new ArgumentOutOfRangeException(nameof(waveformIndex));

            // set default value
            waveformIndexAtEdge = -1;
            edgeType = eEdgeType.NO_EDGE;

            int currentState = waveform[waveformIndex].GetChannelState(channel);
            for (int index = waveformIndex; index < waveform.Count; index++)
            {
                if (waveform[index].GetChannelState(channel) != currentState)
                {
                    edgeType = currentState == 0 ? eEdgeType.RISING_EDGE : eEdgeType.FALLING_EDGE;
                    waveformIndexAtEdge = index;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the previous data edge.
        /// </summary>
        /// <param name="channel">Channel for which to find edge</param>
        /// <param name="waveformIndex">Index from which to start searching</param>
        /// <param name="edgeType">returns the edge type of the found edge. Default is NO_EDGE</param>
        /// <param name="waveformIndexAtEdge">returns the waveform index when the edge is found</param>
        /// <returns>return true when edge is found</returns>
        /// <exception cref="ArgumentOutOfRangeException">Waveform index must be in the range of the waveform</exception>
        public bool GetPreviousEdge(eChannles channel, int waveformIndex, out eEdgeType edgeType, out int waveformIndexAtEdge)
        {
            if (waveformIndex < 0 || waveformIndex > waveform.Count)
                throw new ArgumentOutOfRangeException(nameof(waveformIndex));

            // set default value
            waveformIndexAtEdge = -1;
            edgeType = eEdgeType.NO_EDGE;

            int currentState = waveform[waveformIndex].GetChannelState(channel);
            for (int index = waveformIndex; index >= 0; index--)
            {
                if (waveform[index].GetChannelState(channel) != currentState)
                {
                    edgeType = currentState == 0 ? eEdgeType.FALLING_EDGE : eEdgeType.RISING_EDGE;
                    waveformIndexAtEdge = index;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the edge after specified number of edges
        /// </summary>
        /// <param name="channel">Channel for which to find edge</param>
        /// <param name="waveformIndex">Index from which to start searching</param>
        /// <param name="edgeCount">Number of edges to skip</param>
        /// <param name="edgeType"></param>
        /// <param name="waveformIndexAtEdge">returns the waveform index when the edge is found</param>
        /// <returns>true if edge is found</returns>
        public bool GetNextEdgeByCount(eChannles channel, int waveformIndex, int edgeCount, out eEdgeType edgeType, out int waveformIndexAtEdge)
        {
            // set default value
            waveformIndexAtEdge = -1;
            edgeType = eEdgeType.NO_EDGE;

            for (int counter = 0; counter < edgeCount; counter++)
            {
                if (!GetNextEdge(channel, waveformIndex, out edgeType, out waveformIndexAtEdge))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the edge before specified number of edges
        /// </summary>
        /// <param name="channel">Channel for which to find edge</param>
        /// <param name="waveformIndex">Index from which to start searching</param>
        /// <param name="edgeCount">Number of edges to skip</param>
        /// <param name="edgeType"></param>
        /// <param name="waveformIndexAtEdge">returns the waveform index when the edge is found</param>
        /// <returns>true if edge is found</returns>
        public bool GetPreviousEdgeByCount(eChannles channel, int waveformIndex, int edgeCount, out eEdgeType edgeType, out int waveformIndexAtEdge)
        {
            // set default value
            waveformIndexAtEdge = -1;
            edgeType = eEdgeType.NO_EDGE;

            for (int counter = 0; counter < edgeCount; counter++)
            {
                if (!GetPreviousEdge(channel, waveformIndex, out edgeType, out waveformIndexAtEdge))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the index of the rising edge.
        /// </summary>
        /// <param name="channel">Channel for which to find edge</param>
        /// <param name="waveformIndex">Index from which to start searching</param>
        /// <param name="waveformIndexAtEdge">returns the waveform index when the edge is found</param>
        /// <returns>true if edge is found</returns>
        public bool GetNextRisingEdgeIndex(eChannles channel, int waveformIndex, out int waveformIndexAtEdge)
        {
            if (GetNextEdge(channel, waveformIndex, out eEdgeType edgeType, out waveformIndexAtEdge))
            {
                if (edgeType == eEdgeType.RISING_EDGE)
                    return true;
                else if (GetNextEdge(channel, waveformIndexAtEdge, out edgeType, out waveformIndexAtEdge) && edgeType == eEdgeType.RISING_EDGE)
                    return true;
            }            

            return false;
        }

        /// <summary>
        /// Gets the index of the next falling edge.
        /// </summary>
        /// <param name="channel">Channel for which to find edge</param>
        /// <param name="waveformIndex">Index from which to start searching</param>
        /// <param name="waveformIndexAtEdge">returns the waveform index when the edge is found</param>
        /// <returns>true if edge is found</returns>
        public bool GetNextFallingEdgeIndex(eChannles channel, int waveformIndex, out int waveformIndexAtEdge)
        {
            if (GetNextEdge(channel, waveformIndex, out eEdgeType edgeType, out waveformIndexAtEdge))
            {
                if (edgeType == eEdgeType.FALLING_EDGE)
                    return true;
                else if (GetNextEdge(channel, waveformIndexAtEdge, out edgeType, out waveformIndexAtEdge) && edgeType == eEdgeType.FALLING_EDGE)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get the edge after specified number of edges with specified edge to count
        /// </summary>
        /// <param name="channel">Channel for which to find edge</param>
        /// <param name="waveformIndex">Index from which to start searching</param>
        /// <param name="edgeCount">Number of edges to skip</param>
        /// <param name="edgeToCount">Edge type to count</param>
        /// <param name="waveformIndexAtEdge">returns the waveform index when the edge is found</param>
        /// <returns>true if edge is found</returns>
        public bool GetNextEdgeByCountWithEdge(eChannles channel, int waveformIndex, int edgeCount, eEdgeType edgeToCount, out int waveformIndexAtEdge)
        {
            if (edgeToCount == eEdgeType.NO_EDGE)
                throw new NotSupportedException(nameof(edgeToCount));

            // set default value
            waveformIndexAtEdge = waveformIndex;

            do
            {
                if (!GetNextEdge(channel, waveformIndexAtEdge, out eEdgeType edgeType, out waveformIndexAtEdge))
                    return false;

                if (edgeType == edgeToCount)
                    edgeCount--;

            } while (edgeCount > 0);

            return true;
        }

        /// <summary>
        /// Get the edge before specified number of edges with specified edge to count
        /// </summary>
        /// <param name="channel">Channel for which to find edge</param>
        /// <param name="waveformIndex">Index from which to start searching</param>
        /// <param name="edgeCount">Number of edges to skip</param>
        /// <param name="edgeToCount">Edge type to count</param>
        /// <param name="waveformIndexAtEdge">returns the waveform index when the edge is found</param>
        /// <returns>true if edge is found</returns>
        public bool GetPreviousEdgeByCountWithEdge(eChannles channel, int waveformIndex, int edgeCount, eEdgeType edgeToCount, out int waveformIndexAtEdge)
        {
            if (edgeToCount == eEdgeType.NO_EDGE)
                throw new NotSupportedException(nameof(edgeToCount));

            // set default value
            waveformIndexAtEdge = waveformIndex;

            do
            {
                if (!GetPreviousEdge(channel, waveformIndexAtEdge, out eEdgeType edgeType, out waveformIndexAtEdge))
                    return false;

                if (edgeType == edgeToCount)
                    edgeCount--;

            } while (edgeCount > 0);

            return true;
        }
    }
}
