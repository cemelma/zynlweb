using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Entities;

namespace web.Models
{
    public class HomePageWrapperModel
    {
        public IEnumerable<News> news { get; set; }
        public IEnumerable<References> references { get; set; }
        public IEnumerable<Document> docs { get; set; }
        public IEnumerable<Photo> photos { get; set; }
       
        public IEnumerable<SectorGroup> sectorgroup { get; set; }
        public IEnumerable<ProductGroup> prodgroups { get; set; }
        public IEnumerable<Service> servicegroups { get; set; }
        public Contact contact { get; set; }

        public HomePageWrapperModel()
        {
        }
        public HomePageWrapperModel(Contact contact, IEnumerable<Service> servicegroups, IEnumerable<ProductGroup> prodgroups, IEnumerable<SectorGroup> sectorgroup, IEnumerable<News> news, IEnumerable<References> references, IEnumerable<Document> docs, IEnumerable<Photo> photos)
        {
            this.news = news;
            this.references = references;
            //this.projects = projects;
            this.docs = docs;
            this.photos = photos;
            this.sectorgroup = sectorgroup;
            this.prodgroups = prodgroups;
            this.servicegroups = servicegroups;
            this.contact = contact;
        }
    }
}