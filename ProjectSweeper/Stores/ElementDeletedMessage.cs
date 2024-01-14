using CommunityToolkit.Mvvm.Messaging.Messages;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Stores
{
    public class ElementDeletedMessage : ValueChangedMessage<IEnumerable<IElement>>
    {
        public ElementDeletedMessage(IEnumerable<IElement> value) : base(value)
        {
        }
    }
}
