/*
 *************************************************************************
 * AI for "Connect Five" game.											 *
 *                                                                       *
 * This program should be used for Connect Five Competition.             *
 * Connect Five is the game like Connect Four; for more information see  *
 * http://www.math.spbu.ru/user/chernishev/connectfive/connectfive.html  *
 *                                                                       *
 * Author: Grigory Pervakov                                              *
 * Email: pervakov.grigory@gmail.com									 *
 * Year: 2015                                                            *
 * See the LICENSE file in the project root for more information.        *
 *************************************************************************
*/

using System;
using System.IO;
using System.Collections.Generic;

namespace Brain {

	/// <summary>
	/// With this enum program stores game field
	/// </summary>
	public enum CellState : short {
		Empty = 0, Cross = 1, Zero = -1, Block = 0
	}

	sealed class Program {

		DateTime start;
		Queue<Solution> que = new Queue<Solution>();

		Solution[] branch = new Solution[Field.SIZE];//stores possible move branch 

		/// <summary>
		/// In this constructor main cycle is performed
		/// </summary>
		/// <param name="fold">path to game folder</param>
		/// <param name="player">player symbol</param>
		/// <param name="time">permited time</param>
		public Program(string fold, CellState player, int time) {
			start = DateTime.Now;

			Field field = new Field(fold);
			int delta = 0;
# if DEBUG
			delta = (new Random()).Next(Field.SIZE);//col shift, makes game more different
#endif
			Solution root = new Solution(field, Solution.invertCell(player));
			for (int i = 0; i < Field.SIZE; i++) {
				Solution buf = new Solution(root, (i + delta) % Field.SIZE);
				branch[i] = buf;
				if (!buf.isFinalized)
					que.Enqueue(buf);
			}

			while ((DateTime.Now - start).TotalMilliseconds + 300 < time && que.Count != 0)//main cycle
				Solve(que.Dequeue());

			Solution max = branch[0];

			for (int i = 1; i < Field.SIZE; i++)
				if (branch[i] != null)
					max = branch[i].isGreater(max);

			int turn = Directory.GetFiles(fold).Length / 2 + 1;
			string path = fold + (player == CellState.Cross ? "X" : "O") + turn.ToString() + ".txt";
			File.WriteAllLines(path, new String[] { max.getTurn() });
		}

		/// <summary>
		/// Calculates next branches and adds them to a queue
		/// </summary>
		/// <param name="solve">root of new branches</param>
		void Solve(Solution solve) {
			for (int i = 0; i < Field.SIZE; i++) {
				Solution buf = new Solution(solve, i);
				if (!buf.isFinalized)
					que.Enqueue(buf);
			}
		}

		/// <summary>
		/// This method will be called on startup.
		/// It starts calculating next move.
		/// </summary>
		/// <param name="args">
		/// It needs to have in args:
		/// [0]: path to game field with '/' in the end;
		/// [1]: player symbol 'X' or 'O'
		/// [2]: Permitted time on turn(in mills), should be convertable to int32
		/// </param>
		static void Main(string[] args) {
			try {
				new Program(args[0], args[1][0] == 'X' ? CellState.Cross : CellState.Zero, Convert.ToInt32(args[2]));
			} catch (Exception e) {
				File.WriteAllLines("Exception.txt", new string[] { e.Message });
				throw;
			}
		}
	}

}
