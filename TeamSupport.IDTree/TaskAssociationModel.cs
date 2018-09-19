﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public abstract class TaskAssociationModel : IDNode
    {
        public TaskModel Task { get; protected set; }
        public int TaskAssociationID { get; protected set; }

        public TaskAssociationModel(IDNode node) : base(node)
        {

        }

        private TaskAssociationModel(TaskModel task, int taskAssociationID) : base(task)
        {
            Task = task;
            TaskAssociationID = taskAssociationID;
        }
    }

    class TicketTaskAssociationModel : TaskAssociationModel
    { 
        public TicketModel Ticket { get; private set; }

        private TicketTaskAssociationModel(TicketModel ticket, TaskModel task, int taskAssociationID, bool verify) : base(task)
        {
            Ticket = ticket;
            Task = task;
            if (verify)
                Verify();
        }

        public TicketTaskAssociationModel(TicketModel ticket, TaskModel task, int taskAssociationID) : this(ticket, task, taskAssociationID, true)
        {
        }

        //public static TaskAssociationModel[] GetTaskAssociations(TicketModel ticket)
        //{
        //    int[] taskAssociationIDs = IDReader.Read(TicketChild.TaskAssociations, ticket);
        //    TaskAssociationModel[] taskAssociationModels = new TaskAssociationModel[taskAssociationIDs.Length];
        //    for (int i = 0; i < taskAssociationIDs.Length; ++i)
        //        taskAssociationModels[i] = new TaskAssociationModel(ticket, new TaskNode(ticket.Organization, taskAssociationIDs[i]), false);
        //    return taskAssociationModels;
        //}

        public override void Verify()
        {
            Verify($"SELECT TaskID FROM TaskAssociations WITH (NOLOCK) WHERE TaskID={Task.TaskID} AND Refid={Ticket.TicketID} and RefType = 17");
        }

    }
}
