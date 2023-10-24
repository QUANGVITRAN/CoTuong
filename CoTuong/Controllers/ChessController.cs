using CoTuong.CacheManege;
using CoTuong.Hubs;
using CoTuong.Models;
using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace CoTuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChessController : Controller
    {
        private RoomService roomService;
        private IWebHostEnvironment hostEnvironment;
        private IHubContext<ChatHub> hubContext;

        public ChessController(RoomService roomService, IWebHostEnvironment hostEnvironment, IHubContext<ChatHub> hubContext)
        {
            this.roomService = roomService;
            this.hostEnvironment = hostEnvironment;
            this.hubContext = hubContext;
        }

        [HttpGet]
        [Route("loadchessboard")]
        public IActionResult chessBoard(Guid roomId)
        {
            Room room = roomService.getRoomById(roomId);

            //string chessJson1 = System.IO.File.ReadAllText(hostEnvironment.ContentRootPath + "\\Data\\UpdatedChessJson.txt");
            string chessJson = room.ChessMap;

            // List<ChessNode> chessNodeList1 = JsonSerializer.Deserialize<List<ChessNode>>(chessJson1);
            List<ChessNode> chessNodeList = JsonSerializer.Deserialize<List<ChessNode>>(chessJson);
            List<List<PointModel>> matrix = new List<List<PointModel>>();
            for (int i = 0; i < 10; i++)
            {
                int top = 61 + i * 74;
                List<PointModel> pointList = new List<PointModel>();
                for (int j = 0; j < 9; j++)
                {
                    int left = 106 + j * 74;
                    PointModel p = new PointModel();
                    p.top = top;
                    p.left = left;
                    p.id = "";
                    ChessNode chessNode = chessNodeList.Where(s => s.top == top && s.left == left).FirstOrDefault();
                    if (chessNode != null)
                        p.id = chessNode.id;
                    pointList.Add(p);
                }
                matrix.Add(pointList);
            }
            return Ok(new { status = true, message = "", matrix = matrix, chessNode = chessNodeList });
        }

        [HttpPost]
        [Route("movechess")]
        public IActionResult moveChess(List<MoveChess> moveNodeList)
        {
            MoveChess chess = moveNodeList.FirstOrDefault();
            Room room = roomService.getRoomById(chess.roomId);
            string chessJson = room.ChessMap;
            List<ChessNode> chessNodeList = JsonSerializer.Deserialize<List<ChessNode>>(chessJson);
            foreach (var moveChess in moveNodeList)
            {
                var chessNode = chessNodeList.FirstOrDefault(node => node.id == moveChess.id);

                // Cập nhật các quân cờ vị trí mới của quân cờ

                chessNode.top = moveChess.top;
                chessNode.left = moveChess.left;

                if(chessNode.top == 0)
                {
                    chessNode.visible = "none";
                }
            }
            string updatedChessJson = JsonSerializer.Serialize(chessNodeList);
            room.ChessMap = updatedChessJson;
            room.Turn = chess.turn;
            roomService.update(room);

            hubContext.Clients.All.SendAsync("ReceiveChessMove", JsonSerializer.Serialize(moveNodeList));
            return Ok(new { status = true, message = "" });
        }


    }
}
