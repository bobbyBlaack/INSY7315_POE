using Google.Cloud.Firestore;
using NewDawnProperties.Data;

namespace NewDawnProperties.Services
{
    public class FirestoreSyncService
    {
        private readonly AppDbContext _context;
        private readonly FirestoreDb _firestoreDb;

        public FirestoreSyncService(AppDbContext context, IConfiguration config)
        {
            _context = context;

            // Firestore setup
            string projectId = config["Firebase:ProjectId"];
            string credentialPath = config["Firebase:CredentialPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            _firestoreDb = FirestoreDb.Create(projectId);
        }

        // Full initial sync
        public async Task FullSyncAsync()
        {
            await SyncProperties();
            await SyncLeases();
            await SyncMaintenance();
            await SyncEscalations();
            await SyncRooms();
            await SyncTenantAssignments();
            await SyncCaretakerAssignments();
            await SyncUsers();

            await _context.SaveChangesAsync();
        }

        private async Task SyncProperties()
        {
            foreach (var property in _context.Property)
            {
                var docRef = _firestoreDb.Collection("Properties").Document(property.PropID.ToString());

                await docRef.SetAsync(new
                {
                    property.PropName,
                    property.ListPrice,
                    property.Address,
                    property.City,
                    property.UserID,
                    property.RoomsCount,
                    ListImage = property.ListImage != null ? Convert.ToBase64String(property.ListImage) : null,
                    
                    property.IsSynced
                });

                property.IsSynced = true;
            }
        }

        private async Task SyncLeases()
        {
            foreach (var lease in _context.Leases)
            {
                var docRef = _firestoreDb.Collection("Leases").Document(lease.LeaseID.ToString());

                await docRef.SetAsync(new
                {
                    lease.LeaseStatus,
                    LeaseStart = lease.LeaseStart.ToUniversalTime(), // convert to UTC
                    LeaseEnd = lease.LeaseEnd.ToUniversalTime(),     // convert to UTC
                    lease.RentAmount,
                    lease.UserId,
                    lease.Role,
                    lease.LeaseAction,
                    lease.RoomId,
                    lease.IsSynced
                });

                lease.IsSynced = true;
            }
        }


        private async Task SyncMaintenance()
        {
            foreach (var maintenance in _context.Maintenance)
            {
                var docRef = _firestoreDb.Collection("Maintenance").Document(maintenance.MaintenanceId.ToString());

                await docRef.SetAsync(new
                {
                    maintenance.UserId,
                    maintenance.UserRole,
                    maintenance.Description,
                    maintenance.Type,
                    MaintenanceDate = maintenance.MaintenanceDate.ToUniversalTime(),
                    maintenance.RoomID,
                    maintenance.PropID,
                    maintenance.Status,
                    maintenance.IsSynced
                });

                maintenance.IsSynced = true;
            }
        }

        private async Task SyncEscalations()
        {
            foreach (var escalation in _context.Escalations)
            {
                var docRef = _firestoreDb.Collection("Escalations").Document(escalation.EscalationId.ToString());

                await docRef.SetAsync(new
                {
                    EscalationDate = escalation.EscalationDate.ToUniversalTime(),
                    escalation.RoomId,
                    escalation.UserId,
                    escalation.Category,
                    escalation.Summary,
                    escalation.Actions,
                    escalation.IsSynced
                });

                escalation.IsSynced = true;
            }
        }

        private async Task SyncRooms()
        {
            foreach (var room in _context.Rooms)
            {
                var docRef = _firestoreDb.Collection("Rooms").Document(room.RoomID.ToString());

                await docRef.SetAsync(new
                {
                    room.Block,
                    room.PropID,
                    room.IsSynced
                });

                room.IsSynced = true;
            }
        }

        private async Task SyncTenantAssignments()
        {
            foreach (var tenant in _context.TenantAssignment)
            {
                var docRef = _firestoreDb.Collection("TenantAssignments").Document(tenant.TenantAssignment.ToString());

                await docRef.SetAsync(new
                {
                    tenant.UserID,
                    tenant.PropID,
                    tenant.RoomID,
                    tenant.IsSynced
                });

                tenant.IsSynced = true;
            }
        }

        private async Task SyncCaretakerAssignments()
        {
            foreach (var caretaker in _context.CaretakerAssignment)
            {
                var docRef = _firestoreDb.Collection("CaretakerAssignments")
                                          .Document(caretaker.CaretakerAssignmentID.ToString());

                await docRef.SetAsync(new
                {
                    caretaker.UserID,
                    caretaker.PropID,
                    caretaker.IsSynced
                });

                caretaker.IsSynced = true;
            }
        }

        private async Task SyncUsers()
        {
            foreach (var user in _context.Users)
            {
                var docRef = _firestoreDb.Collection("Users").Document(user.UserID.ToString());

                await docRef.SetAsync(new
                {
                    user.UserName,
                    user.Password,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.FName,
                    user.SName,
                    user.IsSynced
                });

                user.IsSynced = true;
            }
        }

        // Incremental sync example for properties
        public async Task IncrementalSyncAsync()
        {
            var unsyncedProps = _context.Property.Where(p => !p.IsSynced).ToList();
            foreach (var property in unsyncedProps)
            {
                var docRef = _firestoreDb.Collection("Properties").Document(property.PropID.ToString());

                await docRef.SetAsync(new
                {
                    property.PropName,
                    property.ListPrice,
                    property.Address,
                    property.City,
                    property.UserID,
                    property.RoomsCount,
                    ListImage = property.ListImage != null ? Convert.ToBase64String(property.ListImage) : null,
                    ListVideo = property.ListVideo != null ? Convert.ToBase64String(property.ListVideo) : null,
                    property.IsSynced
                });

                property.IsSynced = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
