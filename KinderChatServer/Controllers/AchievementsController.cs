using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using KinderChatServer.DAL;
using KinderChatServer.Models;
using KinderChatServer.Tools;

namespace KinderChatServer.Controllers
{
    [Authorize]
    public class AchievementsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public IQueryable<Achievement> GetAchievements()
        {
            return _db.Achievements;
        }

        [ResponseType(typeof(Achievement))]
        public async Task<IHttpActionResult> GetAchievement(int id)
        {
            Achievement achievement = await _db.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            return Ok(achievement);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAchievement(int id, Achievement achievement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != achievement.Id)
            {
                return BadRequest();
            }

            _db.Entry(achievement).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AchievementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(Achievement))]
        public async Task<IHttpActionResult> PostAchievement(Achievement achievement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Achievements.Add(achievement);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = achievement.Id }, achievement);
        }

        [ResponseType(typeof(Achievement))]
        public async Task<IHttpActionResult> DeleteAchievement(int id)
        {
            Achievement achievement = await _db.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            _db.Achievements.Remove(achievement);
            await _db.SaveChangesAsync();

            return Ok(achievement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AchievementExists(int id)
        {
            return _db.Achievements.Count(e => e.Id == id) > 0;
        }
    }
}