using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyLibrary.PlayerFacade
{
    public class SpectatorCameraRestoreObject
    {
        SpectatorCameraRestoreObjectProperties spectatorCameraRestoreObjectProperties;

        public SpectatorCameraRestoreObject(SpectatorCameraRestoreObjectProperties spectatorCameraRestoreObjectProperties)
        {
            this.spectatorCameraRestoreObjectProperties = spectatorCameraRestoreObjectProperties;
        }

        public string ConstructRestoreObject(string name)
        {
            spectatorCameraRestoreObjectProperties.gridInfos = spectatorCameraRestoreObjectProperties.gridInfos.OrderBy(item => item.gridProperties.name).ToList();
            // Tìm kiếm phần tử có tên tương ứng
            GridInfo gridInfo = FindByName(name);
            if (gridInfo == null)
            {
                return FindGrid(name);
            }
            else
            {
                // Kiểm tra số lượng và xử lý
                if (gridInfo.gridProperties.amount >= 10)
                {
                    return FindGrid(name);
                }

                gridInfo.gridProperties.amount++;
            }

            return "Got it!";
        }

    private string FindGrid(string name)
    {
        // Tìm phần tử đầu tiên có GridProperties với name rỗng hoặc null
        var firstInvalidItem = spectatorCameraRestoreObjectProperties.gridInfos.FirstOrDefault(item => string.IsNullOrEmpty(item.gridProperties.name));

        if (firstInvalidItem == null)
            return "Ooops! There are no more empty cells.";

        // Cập nhật tên và tăng số lượng
        firstInvalidItem.gridProperties.name = name;
        firstInvalidItem.gridProperties.amount++;
        return "Luckily there is still a slot left.";
    }

        public GridInfo FindByName(string name) => spectatorCameraRestoreObjectProperties.gridInfos.FirstOrDefault(item => item.gridProperties.name == name);
    }

    [System.Serializable]
    public class SpectatorCameraRestoreObjectProperties
    {
        public List<GridInfo> gridInfos; 
    }
}

