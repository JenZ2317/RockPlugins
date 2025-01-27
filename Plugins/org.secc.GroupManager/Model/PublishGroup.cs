﻿// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using Rock.Data;
using Rock.Model;
using Rock.Security;

namespace org.secc.GroupManager.Model
{
    [Table( "_org_secc_GroupManager_PublishGroup" )]
    [DataContract]
    public class PublishGroup : Model<PublishGroup>, ISecured, IRockEntity
    {
        [Index]
        [DataMember]
        public int GroupId { get; set; }

        [LavaInclude]
        public virtual Group Group { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int? ImageId { get; set; }

        [LavaInclude]
        public BinaryFile Image { get; set; }

        [Index]
        [DataMember]
        public DateTime StartDateTime { get; set; }

        [Index]
        [DataMember]
        public DateTime EndDateTime { get; set; }

        [LavaInclude]
        public virtual ICollection<DefinedValue> AudienceValues
        {
            get { return _audienceValues ?? ( _audienceValues = new Collection<DefinedValue>() ); }
            set { _audienceValues = value; }
        }
        private ICollection<DefinedValue> _audienceValues;

        [Index]
        [DataMember]
        public int RequestorAliasId { get; set; }

        [LavaInclude]
        public virtual PersonAlias RequestorAlias { get; set; }

        [Index]
        [DataMember]
        public int ContactPersonAliasId { get; set; }

        [LavaInclude]
        public virtual PersonAlias ContactPersonAlias { get; set; }

        [DataMember]
        public string ContactEmail { get; set; }

        [DataMember]
        public bool RequiresRegistration { get; set; } = false;

        [DataMember]
        public string RegistrationLink { get; set; }

        [DataMember]
        public bool ChildcareAvailable { get; set; } = false;

        [DataMember]
        public string ChildcareRegistrationLink { get; set; }

        [DataMember]
        public string ContactPhoneNumber { get; set; }

        [DataMember]
        public string ConfirmationFromName { get; set; }

        [DataMember]
        public string ConfirmationEmail { get; set; }

        [DataMember]
        public string ConfirmationSubject { get; set; }

        [DataMember]
        public string ConfirmationBody { get; set; }

        [DataMember]
        public PublishGroupStatus PublishGroupStatus { get; set; }

        [DataMember]
        public bool AllowSpouseRegistration { get; set; }

        [DataMember]
        public DayOfWeek? WeeklyDayOfWeek { get; set; }

        [DataMember]
        public TimeSpan? WeeklyTimeOfDay { get; set; }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public string MeetingLocation { get; set; }

        [NotMapped]
        public bool IsActive { get => WasActive( Rock.RockDateTime.Today ); }

        [LavaInclude]
        public bool WasActive( DateTime dateTime )
        {

            return StartDateTime <= dateTime && EndDateTime >= dateTime;
        }

        [LavaInclude]
        public bool IsFull
        {
            get
            {

                if ( Group.GroupType.GroupCapacityRule == GroupCapacityRule.None
                   || !Group.GroupCapacity.HasValue )
                {
                    return false;
                }
                else
                {
                    return Group.GroupCapacity < Group.ActiveMembers().Count();
                }
            }
        }

        [LavaInclude]
        public bool IsNotFull { get => !IsFull; }
    }

    public enum PublishGroupStatus
    {
        Pending = 0,
        Approved = 1,
        Denied = 2
    }

    public partial class PublishGroupConfiguration : EntityTypeConfiguration<PublishGroup>
    {
        public PublishGroupConfiguration()
        {
            this.HasMany( pg => pg.AudienceValues ).WithMany().Map( pg => { pg.MapLeftKey( "PublishGroupdId" ); pg.MapRightKey( "DefinedValueId" ); pg.ToTable( "_org_secc_GroupManager_PublishGroupAudienceValue" ); } );
            this.HasEntitySetName( "PublishGroups" );
        }
    }
}
