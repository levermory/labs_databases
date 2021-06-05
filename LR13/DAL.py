from pymongo import MongoClient
import pprint
import json


class MongoCRUD:
    def __init__(self):
        self.client = MongoClient("mongodb://localhost:27017")
        self.db = self.client.students
        self.students_collection = self.db.students

    def show_all(self):
        for doc in self.students_collection.find():
            pprint.pprint(doc, sort_dicts=False)
    

    def show_student(self, *docs: dict):
        for doc in self.students_collection.find(*docs):
            pprint.pprint(doc, sort_dicts=False)


    def add_student(self, doc: dict):
        self.students_collection.insert_one(doc)


    def add_stud_from_file(self, path: str): 
        with open(path) as json_inp_file:
            data = json.load(json_inp_file)
            for doc in data:
                self.add_student(doc)

    def del_student(self, doc: dict):
        self.students_collection.delete_one(doc)


    def export_studs_to_file(self, path: str): 
        docs = self.students_collection.find()
        res = []
        for doc in docs:
            res.append(doc.copy())
        with open(path, 'w') as json_out_file:
            json_out_file.write(json.dumps(res, default=str, sort_keys=True, indent=4))
    

    
            

    def close(self): self.client.close()


from DAL import MongoCRUD

if __name__ == '__main__':
    client = MongoCRUD()
    client.show_student({"Name": "Edgar"}, {"_id": False})
    client.show_all()
    # client.add_stud_from_file("new_loh.json")
    client.del_student({"Name":"Elya Loh"})
    client.show_all()
    client.export_studs_to_file("vse_lohi.json")
    query = client.students_collection.aggregate([{"$group":{"_id": "Speciality", "Studs":{"$sum":1}}}])
    for i in query:
        pprint.pprint(i)
    
    client.close()
