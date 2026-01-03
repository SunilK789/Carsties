using AuctionService.Data;
using AuctionService.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await _context.Auctions
                .Include(a => a.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();
            var auctionDtos = _mapper.Map<List<AuctionDto>>(auctions);
            return Ok(auctionDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions
                .Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            return Ok(auctionDto);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = _mapper.Map<Entities.Auction>(createAuctionDto);
            auction.Id = Guid.NewGuid();
            //auction.Seller = "system"; // In a real app, get the seller from the authenticated user
            auction.CreatedAt = DateTime.UtcNow;
            auction.UpdatedAt = DateTime.UtcNow;
            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auctionDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, CreateAuctionDto updateAuctionDto)
        {
            var auction = await _context.Auctions
                .Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            _mapper.Map(updateAuctionDto, auction);
            auction.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var auctionDto = _mapper.Map<AuctionDto>(auction);
            return Ok(auctionDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
   
    
