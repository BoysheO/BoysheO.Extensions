// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

var players = Enumerable.Range(1, 8).ToList();
Dictionary<int, int> player2lastPlayer = new();
int last_missing_player = 0; //上一轮中轮空的玩家
//40回合
for (int i = 0; i < 40; i++)
{
    if (i >= 20 && i % 10 == 0)
    {
        //随机淘汰一个
        players.RemoveAt(Random.Shared.Next(players.Count));
    }

    List<int> orders = new();
    while (players.Count > 0)
    {
        int p1;
        if (last_missing_player != 0)
        {
            p1 = last_missing_player;
            players.Remove(p1);
            last_missing_player = 0;
        }
        else
        {
            p1 = players[0];
            players.RemoveAt(0);
        }

        orders.Add(p1);

        //如果玩家列表没玩家了，则轮空
        if (players.Count == 0)
        {
            last_missing_player = p1;
            continue;
        }

        var lastP2 = player2lastPlayer.GetValueOrDefault(p1);
        var pool = players.Where(v => v != lastP2).ToArray();
        var p2 = pool[Random.Shared.Next(pool.Length)]; //抽一个p2
        players.Remove(p2);
        orders.Add(p2);
    }

    //打印匹配信息
    Console.Write($"第{i + 1}回合");
    for (int j = 0; j < orders.Count; j += 2)
    {
        var p1 = orders[j];
        var p2Idx = j + 1;
        if (p2Idx >= orders.Count)
        {
            Console.Write($"轮空:p{p1}");
        }
        else
        {
            var p2 = orders[j + 1];
            Console.Write($"{p1}-{p2}");
        }

        Console.Write(" ");
    }

    Console.WriteLine();
    //打印完毕之后要还原player列表
    players = orders;
}