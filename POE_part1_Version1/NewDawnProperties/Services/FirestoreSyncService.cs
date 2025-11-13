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
            // Property table
            foreach (var property in _context.Property)
            {
                var docRef = _firestoreDb.Collection("Properties")
                                          .Document(property.PropID.ToString());

                await docRef.SetAsync(new
                {
                    property.PropName,
                    property.ListPrice,
                    property.Address,
                    property.City,
                    property.UserID,
                    property.RoomsCount,
                    ListImage = Convert.ToBase64String(property.ListImage),
                    ListVideo = Convert.ToBase64String(property.ListVideo)
                });

                property.IsSynced = true;
            }

            // Leases
            foreach (var lease in _context.Leases)
            {
                var docRef = _firestoreDb.Collection("Leases")
                                          .Document(lease.LeaseID.ToString());

                await docRef.SetAsync(new
                {
                    lease.LeaseStatus,
                    lease.LeaseStart,
                    lease.LeaseEnd,
                    lease.RentAmount,
                    lease.UserId,
                    lease.Role,
                    lease.LeaseAction,
                    lease.RoomId
                });

                lease.IsSynced = true;
            }

            // Maintenance
            foreach (var maintenance in _context.Maintenance)
            {
                var docRef = _firestoreDb.Collection("Maintenance")
                                          .Document(maintenance.MaintenanceId.ToString());

                await docRef.SetAsync(new
                {
                    maintenance.UserId,
                    maintenance.UserRole,
                    maintenance.Description,
                    maintenance.Type,
                    maintenance.MaintenanceDate,
                    maintenance.RoomID,
                    maintenance.PropID,
                    maintenance.Status
                });

                maintenance.IsSynced = true;
            }

            // Escalations
            foreach (var escalation in _context.Escalations)
            {
                var docRef = _firestoreDb.Collection("Escalations")
                                          .Document(escalation.EscalationId.ToString());

                await docRef.SetAsync(new
                {
                    escalation.EscalationDate,
                    escalation.RoomId,
                    escalation.UserId,
                    escalation.Category,
                    escalation.Summary,
                    escalation.Actions
                });

                escalation.IsSynced = true;
            }

            // Rooms
            foreach (var room in _context.Rooms)
            {
                var docRef = _firestoreDb.Collection("Rooms")
                                          .Document(room.RoomID.ToString());

                await docRef.SetAsync(new
                {
                    room.Block,
                    room.PropID
                });

                room.IsSynced = true;
            }

            // TenantAssignment
            foreach (var tenant in _context.TenantAssignment)
            {
                var docRef = _firestoreDb.Collection("TenantAssignments")
                                          .Document(tenant.TenantAssignment.ToString());

                await docRef.SetAsync(new
                {
                    tenant.UserID,
                    tenant.PropID,
                    tenant.RoomID
                });

                tenant.IsSynced = true;
            }

            // CaretakerAssignment
            foreach (var caretaker in _context.CaretakerAssignment)
            {
                var docRef = _firestoreDb.Collection("CaretakerAssignments")
                                          .Document(caretaker.CaretakerAssignmentID.ToString());

                await docRef.SetAsync(new
                {
                    caretaker.UserID,
                    caretaker.PropID
                });

                caretaker.IsSynced = true;
            }

            // Users
            foreach (var user in _context.Users)
            {
                var docRef = _firestoreDb.Collection("Users")
                                          .Document(user.UserID.ToString());

                await docRef.SetAsync(new
                {
                    user.UserName,
                    user.Password,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.FName,
                    user.SName
                });

                user.IsSynced = true;
            }

            // Save all changes to mark as synced
            await _context.SaveChangesAsync();
        }


        // Incremental sync
        public async Task IncrementalSyncAsync()
        {
            var unsyncedProps = _context.Property.Where(p => !p.IsSynced).ToList();
            foreach (var property in unsyncedProps)
            {
                DocumentReference docRef = _firestoreDb.Collection("Properties")
                                                      .Document(property.PropID.ToString());

                await docRef.SetAsync(new
                {
                    property.PropName,
                    property.ListPrice,
                    property.Address,
                    property.City,
                    property.UserID,
                    property.RoomsCount,
                    ListImage = Convert.ToBase64String(property.ListImage),
                    ListVideo = Convert.ToBase64String(property.ListVideo)
                });

                property.IsSynced = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
