using Hik.Communication.ScsServices.Service;
using LIFE.API.Environment.GeoCommon;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.API.Layer.Obstacle;
using LIFE.Components.Utilities.CloudSupport;

namespace LIFE.Components.ObstacleLayer
{
    public abstract class ObstacleLayer : ScsService, IObstacleLayer
    {
        private long _currentTick;
        private IObstacleMap _obstacleMap;

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            var fileId = layerInitData.FileInitInfo.FileId;
            var filePath = new FileClient().DownloadFile(fileId);
            _obstacleMap = new ObstacleMapLoader().LoadObstacleMap(filePath);
            return true;
        }

        public double GetAccumulatedPathRating(IGeoCoordinate start, IGeoCoordinate destination,
            double failFastThreshold = double.MaxValue)
        {
            return _obstacleMap.GetAccumulatedPathRating(start, destination, failFastThreshold);
        }

        public double GetAccumulatedPathRating(IGeoCoordinate start, double speed, double bearing,
            double failFastThreshold = double.MaxValue)
        {
            return _obstacleMap.GetAccumulatedPathRating(start, speed, bearing, failFastThreshold);
        }

        public void AddCellRating(IGeoCoordinate position, double ratingValue)
        {
            _obstacleMap.AddCellRating(position, ratingValue);
        }

        public void ReduceCellRating(IGeoCoordinate position, double ratingValue)
        {
            _obstacleMap.ReduceCellRating(position, ratingValue);
        }

        public double TryToReduceCellRating(IGeoCoordinate position, double amountToTake)
        {
            return _obstacleMap.TryTakeFromCell(position, amountToTake);
        }

        public double GetCellRating(IGeoCoordinate position)
        {
            return _obstacleMap.GetCellRating(position);
        }

        public IGeoCoordinate GetNeighbourCellWithMaxValue(IGeoCoordinate positon)
        {
            return _obstacleMap.GetMaxValueNeighbourCoordinate(positon);
        }

        public void SetCellRating(IGeoCoordinate position, double cellValue)
        {
            _obstacleMap.SetCellRating(position, cellValue);
        }

        public long GetCurrentTick()
        {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick)
        {
            _currentTick = currentTick;
        }
    }
}